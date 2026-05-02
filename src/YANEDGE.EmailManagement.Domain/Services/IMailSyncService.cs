namespace YANEDGE.EmailManagement.Domain.Services;

/// <summary>
/// 邮件同步服务接口
/// </summary>
public interface IMailSyncService
{
    /// <summary>
    /// 触发邮件同步任务
    /// </summary>
    Task<MailSyncJobResult> TriggerSyncAsync(Guid mailAccountId);
}

/// <summary>
/// 邮件同步任务结果
/// </summary>
public class MailSyncJobResult
{
    /// <summary>
    /// 任务ID
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// 是否已接受
    /// </summary>
    public bool Accepted { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; }
}
