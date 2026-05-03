using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Domain.Services.Implementations;

/// <summary>
/// 邮件同步服务实现
/// </summary>
public class MailSyncService : IMailSyncService, ITransientDependency
{
    private readonly IRepository<MailAccount.MailAccount, Guid> _mailAccountRepository;

    public MailSyncService(IRepository<MailAccount.MailAccount, Guid> mailAccountRepository)
    {
        _mailAccountRepository = mailAccountRepository;
    }

    public async Task<MailSyncJobResult> TriggerSyncAsync(Guid mailAccountId)
    {
        // 验证账号存在
        var account = await _mailAccountRepository.FindAsync(mailAccountId);
        if (account == null)
        {
            return new MailSyncJobResult
            {
                JobId = Guid.Empty,
                Accepted = false,
                Message = "邮箱账号不存在"
            };
        }

        if (!account.SyncEnabled)
        {
            return new MailSyncJobResult
            {
                JobId = Guid.Empty,
                Accepted = false,
                Message = "邮箱同步未启用"
            };
        }

        // TODO: 集成Hangfire后，使用BackgroundJobManager排队后台任务
        // var jobId = await _backgroundJobManager.EnqueueAsync(
        //     new MailSyncJobArgs { MailAccountId = mailAccountId });

        var jobId = Guid.NewGuid();

        return new MailSyncJobResult
        {
            JobId = jobId,
            Accepted = true,
            Message = "同步任务已排队（待集成Hangfire后实现实际同步）"
        };
    }
}
