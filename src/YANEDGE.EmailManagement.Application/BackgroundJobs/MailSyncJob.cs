using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using YANEDGE.EmailManagement.Domain.MailAccount;
using YANEDGE.EmailManagement.Domain.Services;

namespace YANEDGE.EmailManagement.Application.BackgroundJobs;

/// <summary>
/// 邮件同步后台任务
/// </summary>
public class MailSyncJob : ITransientDependency
{
    private readonly IMailAccountRepository _mailAccountRepository;
    private readonly IMailSyncService _mailSyncService;
    private readonly ILogger<MailSyncJob> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    // Metrics constants for monitoring
    private const string JobName = "MailSync";

    public MailSyncJob(
        IMailAccountRepository mailAccountRepository,
        IMailSyncService mailSyncService,
        ILogger<MailSyncJob> logger,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _mailAccountRepository = mailAccountRepository;
        _mailSyncService = mailSyncService;
        _logger = logger;
        _unitOfWorkManager = unitOfWorkManager;
    }

    /// <summary>
    /// 执行邮件同步任务
    /// </summary>
    public async Task ExecuteAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("[{JobName}] 邮件同步任务开始执行...", JobName);

        var successCount = 0;
        var failCount = 0;
        var totalAccounts = 0;

        try
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            // 获取所有启用同步的邮箱账号
            var accounts = await _mailAccountRepository.GetSyncEnabledAccountsAsync();
            totalAccounts = accounts.Count;

            if (totalAccounts == 0)
            {
                _logger.LogInformation("[{JobName}] 没有启用同步的邮箱账号", JobName);
                await uow.CompleteAsync();
                return;
            }

            _logger.LogInformation("[{JobName}] 找到 {Count} 个启用同步的邮箱账号", JobName, totalAccounts);

            // 使用并发控制处理账号，避免过载
            var semaphore = new SemaphoreSlim(3); // 限制最多3个并发同步
            var tasks = new List<Task>();

            foreach (var account in accounts)
            {
                await semaphore.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogDebug("[{JobName}] 开始同步邮箱: {Email} (ID: {AccountId})",
                            JobName, account.EmailAddress, account.Id);

                        await _mailSyncService.TriggerSyncAsync(account.Id);

                        Interlocked.Increment(ref successCount);
                        _logger.LogDebug("[{JobName}] 邮箱同步成功: {Email}", JobName, account.EmailAddress);
                    }
                    catch (OperationCanceledException)
                    {
                        Interlocked.Increment(ref failCount);
                        _logger.LogWarning("[{JobName}] 邮箱同步已取消: {Email}", JobName, account.EmailAddress);
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref failCount);
                        _logger.LogError(ex, "[{JobName}] 邮箱同步失败: {Email} (ID: {AccountId})",
                            JobName, account.EmailAddress, account.Id);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
            await uow.CompleteAsync();

            stopwatch.Stop();
            var successRate = totalAccounts > 0 ? (double)successCount / totalAccounts * 100 : 0;

            _logger.LogInformation(
                "[{JobName}] 邮件同步任务执行完成。总计: {Total}, 成功: {Success}, 失败: {Fail}, 成功率: {Rate:F2}%, 耗时: {Duration}ms",
                JobName, totalAccounts, successCount, failCount, successRate, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[{JobName}] 邮件同步任务执行出错，耗时: {Duration}ms",
                JobName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
