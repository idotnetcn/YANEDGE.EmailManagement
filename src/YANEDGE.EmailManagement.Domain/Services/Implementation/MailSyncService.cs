using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using YANEDGE.EmailManagement.Services;
using YANEDGE.EmailManagement.Domain.Services;
using YANEDGE.EmailManagement.Domain.MailAccount;
using YANEDGE.EmailManagement.Domain.MailMessage;

namespace YANEDGE.EmailManagement.Services.Implementation;

/// <summary>
/// 邮件同步服务实现
/// </summary>
public class MailSyncService : IMailSyncService, ITransientDependency
{
    private const string SyncLockCacheKeyPrefix = "mail-sync:lock:";
    private const string SyncStatusCacheKeyPrefix = "mail-sync:status:";
    private static readonly TimeSpan SyncLockTtl = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan SyncStatusTtl = TimeSpan.FromHours(12);

    private readonly IMailAccountRepository _mailAccountRepository;
    private readonly IMailMessageRepository _mailMessageRepository;
    private readonly IMailProtocolAdapter _protocolAdapter;
    private readonly ILogger<MailSyncService> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ICacheService _cacheService;

    public MailSyncService(
        IMailAccountRepository mailAccountRepository,
        IMailMessageRepository mailMessageRepository,
        IMailProtocolAdapter protocolAdapter,
        ILogger<MailSyncService> logger,
        IUnitOfWorkManager unitOfWorkManager,
        IBackgroundJobClient backgroundJobClient,
        ICacheService cacheService)
    {
        _mailAccountRepository = mailAccountRepository;
        _mailMessageRepository = mailMessageRepository;
        _protocolAdapter = protocolAdapter;
        _logger = logger;
        _unitOfWorkManager = unitOfWorkManager;
        _backgroundJobClient = backgroundJobClient;
        _cacheService = cacheService;
    }

    public async Task<MailSyncJobResult> TriggerSyncAsync(Guid mailAccountId)
    {
        var jobId = Guid.NewGuid();

        try
        {
            var account = await _mailAccountRepository.GetAsync(mailAccountId);

            if (!account.SyncEnabled)
            {
                _logger.LogWarning("Sync is disabled for account {AccountId}", mailAccountId);
                return new MailSyncJobResult
                {
                    JobId = jobId,
                    Accepted = false,
                    Status = MailSyncExecutionStatus.Skipped,
                    Message = "Sync is disabled for this account"
                };
            }

            var syncLockKey = GetSyncLockCacheKey(mailAccountId);
            var existingLock = await _cacheService.GetAsync<MailSyncStatusSnapshot>(syncLockKey);
            if (existingLock != null)
            {
                _logger.LogInformation(
                    "Mail sync skipped because another sync is already running for account {AccountId}, existing job {ExistingJobId}",
                    mailAccountId,
                    existingLock.JobId);

                return new MailSyncJobResult
                {
                    JobId = existingLock.JobId,
                    Accepted = false,
                    BackgroundJobId = existingLock.BackgroundJobId,
                    Status = existingLock.Status,
                    Message = $"A sync job is already in progress for account {account.EmailAddress}"
                };
            }

            var statusSnapshot = new MailSyncStatusSnapshot
            {
                JobId = jobId,
                MailAccountId = mailAccountId,
                Status = MailSyncExecutionStatus.Pending,
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(SyncStatusTtl),
                Message = "Mail sync request accepted"
            };

            await _cacheService.SetAsync(syncLockKey, statusSnapshot, SyncLockTtl);
            await _cacheService.SetAsync(GetSyncStatusCacheKey(jobId), statusSnapshot, SyncStatusTtl);

            var backgroundJobId = _backgroundJobClient.Enqueue<IMailSyncService>(service => service.ExecuteSyncAsync(mailAccountId, jobId));

            statusSnapshot.BackgroundJobId = backgroundJobId;
            statusSnapshot.Status = MailSyncExecutionStatus.Queued;
            statusSnapshot.UpdatedAt = DateTime.UtcNow;
            statusSnapshot.Message = $"Mail sync job queued for account {account.EmailAddress}";

            await _cacheService.SetAsync(syncLockKey, statusSnapshot, SyncLockTtl);
            await _cacheService.SetAsync(GetSyncStatusCacheKey(jobId), statusSnapshot, SyncStatusTtl);

            _logger.LogInformation(
                "Mail sync queued for account {AccountId}, sync job {JobId}, background job {BackgroundJobId}",
                mailAccountId, jobId, backgroundJobId);

            return new MailSyncJobResult
            {
                JobId = jobId,
                Accepted = true,
                BackgroundJobId = backgroundJobId,
                Status = MailSyncExecutionStatus.Queued,
                Message = "Mail sync job queued successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mail sync failed for account {AccountId}, job {JobId}", mailAccountId, jobId);

            return new MailSyncJobResult
            {
                JobId = jobId,
                Accepted = false,
                Status = MailSyncExecutionStatus.Failed,
                Message = $"Mail sync failed: {ex.Message}"
            };
        }
    }

    public async Task ExecuteSyncAsync(Guid mailAccountId, Guid jobId)
    {
        var statusCacheKey = GetSyncStatusCacheKey(jobId);
        var lockCacheKey = GetSyncLockCacheKey(mailAccountId);
        var startedAt = DateTime.UtcNow;

        try
        {
            _logger.LogInformation("Starting queued mail sync for account {AccountId}, job {JobId}", mailAccountId, jobId);

            var statusSnapshot = await GetOrCreateStatusSnapshotAsync(mailAccountId, jobId);
            statusSnapshot.Status = MailSyncExecutionStatus.Running;
            statusSnapshot.StartedAt = startedAt;
            statusSnapshot.UpdatedAt = startedAt;
            statusSnapshot.Message = "Mail sync is running";

            await _cacheService.SetAsync(lockCacheKey, statusSnapshot, SyncLockTtl);
            await _cacheService.SetAsync(statusCacheKey, statusSnapshot, SyncStatusTtl);

            var account = await _mailAccountRepository.GetAsync(mailAccountId);
            DateTime? sinceDate = account.LastSyncAt ?? DateTime.UtcNow.AddDays(-30);

            using var uow = _unitOfWorkManager.Begin(requiresNew: true);

            var messages = await _protocolAdapter.SyncMailsAsync(account, sinceDate);
            var savedCount = 0;

            foreach (var message in messages)
            {
                if (!string.IsNullOrEmpty(message.InternetMessageId))
                {
                    var existing = await _mailMessageRepository.FindByInternetMessageIdAsync(message.InternetMessageId);
                    if (existing != null)
                    {
                        _logger.LogDebug("Message {MessageId} already exists, skipping", message.InternetMessageId);
                        continue;
                    }
                }

                await _mailMessageRepository.InsertAsync(message, autoSave: false);
                savedCount++;
            }

            account.UpdateLastSyncTime(DateTime.UtcNow);
            await _mailAccountRepository.UpdateAsync(account, autoSave: false);

            await uow.CompleteAsync();

            statusSnapshot.Status = MailSyncExecutionStatus.Completed;
            statusSnapshot.UpdatedAt = DateTime.UtcNow;
            statusSnapshot.CompletedAt = statusSnapshot.UpdatedAt;
            statusSnapshot.RetrievedCount = messages.Count;
            statusSnapshot.SavedCount = savedCount;
            statusSnapshot.Message = $"Mail sync completed successfully. Retrieved: {messages.Count}, Saved: {savedCount}";
            statusSnapshot.ErrorMessage = null;

            await _cacheService.SetAsync(statusCacheKey, statusSnapshot, SyncStatusTtl);
            await _cacheService.RemoveAsync(lockCacheKey);

            _logger.LogInformation(
                "Mail sync completed for account {AccountId}. Retrieved: {Retrieved}, Saved: {Saved}, job {JobId}",
                mailAccountId,
                messages.Count,
                savedCount,
                jobId);
        }
        catch (Exception ex)
        {
            var failedSnapshot = await GetOrCreateStatusSnapshotAsync(mailAccountId, jobId);
            failedSnapshot.Status = MailSyncExecutionStatus.Failed;
            failedSnapshot.UpdatedAt = DateTime.UtcNow;
            failedSnapshot.CompletedAt = failedSnapshot.UpdatedAt;
            failedSnapshot.ErrorMessage = ex.Message;
            failedSnapshot.Message = $"Mail sync failed: {ex.Message}";

            await _cacheService.SetAsync(statusCacheKey, failedSnapshot, SyncStatusTtl);
            await _cacheService.RemoveAsync(lockCacheKey);

            _logger.LogError(ex, "Queued mail sync failed for account {AccountId}, job {JobId}", mailAccountId, jobId);
            throw;
        }
    }

    private async Task<MailSyncStatusSnapshot> GetOrCreateStatusSnapshotAsync(Guid mailAccountId, Guid jobId)
    {
        return await _cacheService.GetAsync<MailSyncStatusSnapshot>(GetSyncStatusCacheKey(jobId))
            ?? new MailSyncStatusSnapshot
            {
                JobId = jobId,
                MailAccountId = mailAccountId,
                Status = MailSyncExecutionStatus.Pending,
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(SyncStatusTtl)
            };
    }

    private static string GetSyncLockCacheKey(Guid mailAccountId)
    {
        return $"{SyncLockCacheKeyPrefix}{mailAccountId:N}";
    }

    private static string GetSyncStatusCacheKey(Guid jobId)
    {
        return $"{SyncStatusCacheKeyPrefix}{jobId:N}";
    }
}
