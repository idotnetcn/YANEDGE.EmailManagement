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
        _logger.LogInformation("邮件同步任务开始执行...");

        try
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            // 获取所有启用同步的邮箱账号
            var accounts = await _mailAccountRepository.GetSyncEnabledAccountsAsync();

            _logger.LogInformation("找到 {Count} 个启用同步的邮箱账号", accounts.Count);

            var successCount = 0;
            var failCount = 0;

            foreach (var account in accounts)
            {
                try
                {
                    _logger.LogDebug("开始同步邮箱: {Email}", account.EmailAddress);

                    await _mailSyncService.TriggerSyncAsync(account.Id);

                    successCount++;
                    _logger.LogDebug("邮箱同步成功: {Email}", account.EmailAddress);
                }
                catch (Exception ex)
                {
                    failCount++;
                    _logger.LogError(ex, "邮箱同步失败: {Email}", account.EmailAddress);
                }
            }

            await uow.CompleteAsync();

            _logger.LogInformation(
                "邮件同步任务执行完成。成功: {Success}, 失败: {Fail}",
                successCount,
                failCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "邮件同步任务执行出错");
            throw;
        }
    }
}
