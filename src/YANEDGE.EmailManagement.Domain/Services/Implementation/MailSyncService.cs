using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using YANEDGE.EmailManagement.Domain.Services;
using YANEDGE.EmailManagement.Domain.MailAccount;
using YANEDGE.EmailManagement.Domain.MailMessage;

namespace YANEDGE.EmailManagement.Services.Implementation;

/// <summary>
/// 邮件同步服务实现
/// </summary>
public class MailSyncService : IMailSyncService, ITransientDependency
{
    private readonly IMailAccountRepository _mailAccountRepository;
    private readonly IMailMessageRepository _mailMessageRepository;
    private readonly IMailProtocolAdapter _protocolAdapter;
    private readonly ILogger<MailSyncService> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public MailSyncService(
        IMailAccountRepository mailAccountRepository,
        IMailMessageRepository mailMessageRepository,
        IMailProtocolAdapter protocolAdapter,
        ILogger<MailSyncService> logger,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _mailAccountRepository = mailAccountRepository;
        _mailMessageRepository = mailMessageRepository;
        _protocolAdapter = protocolAdapter;
        _logger = logger;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<MailSyncJobResult> TriggerSyncAsync(Guid mailAccountId)
    {
        var jobId = Guid.NewGuid();

        try
        {
            _logger.LogInformation("Starting mail sync for account {AccountId}, job {JobId}", mailAccountId, jobId);

            // 获取邮箱账号
            var account = await _mailAccountRepository.GetAsync(mailAccountId);

            if (!account.SyncEnabled)
            {
                _logger.LogWarning("Sync is disabled for account {AccountId}", mailAccountId);
                return new MailSyncJobResult
                {
                    JobId = jobId,
                    Accepted = false,
                    Message = "Sync is disabled for this account"
                };
            }

            // 使用单独的工作单元进行同步操作
            using var uow = _unitOfWorkManager.Begin(requiresNew: true);

            // 获取上次同步时间，如果没有则获取最近30天的邮件
            DateTime? sinceDate = account.LastSyncAt ?? DateTime.UtcNow.AddDays(-30);

            // 通过协议适配器同步邮件
            var messages = await _protocolAdapter.SyncMailsAsync(account, sinceDate);

            // 保存新邮件到数据库
            var savedCount = 0;
            foreach (var message in messages)
            {
                // 检查邮件是否已存在（根据 InternetMessageId）
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

            // 更新账号的最后同步时间
            account.UpdateLastSyncTime(DateTime.UtcNow);
            await _mailAccountRepository.UpdateAsync(account, autoSave: false);

            // 提交工作单元
            await uow.CompleteAsync();

            _logger.LogInformation(
                "Mail sync completed for account {AccountId}. Retrieved: {Retrieved}, Saved: {Saved}",
                mailAccountId, messages.Count, savedCount);

            return new MailSyncJobResult
            {
                JobId = jobId,
                Accepted = true,
                Message = $"Mail sync completed successfully. Retrieved: {messages.Count}, Saved: {savedCount}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mail sync failed for account {AccountId}, job {JobId}", mailAccountId, jobId);

            return new MailSyncJobResult
            {
                JobId = jobId,
                Accepted = false,
                Message = $"Mail sync failed: {ex.Message}"
            };
        }
    }
}
