using YANEDGE.EmailManagement.Domain.MailCompose;

namespace YANEDGE.EmailManagement.Domain.Services;

/// <summary>
/// 邮件发送服务接口
/// </summary>
public interface IMailSendService
{
    /// <summary>
    /// 排队发送邮件
    /// </summary>
    Task QueueSendTaskAsync(MailSendTask sendTask);

    /// <summary>
    /// 实际执行邮件发送(后台任务调用)
    /// </summary>
    Task ExecuteSendTaskAsync(Guid sendTaskId);
}
