using System;
using Volo.Abp.EventBus;

namespace YANEDGE.EmailManagement.Domain.Shared.Events;

/// <summary>
/// 发送任务失败事件
/// Event triggered when a mail send task fails
/// </summary>
[EventName("EmailManagement.MailSendTask.Failed")]
public class MailSendTaskFailedEvent 
{
    public Guid SendTaskId { get; set; }
    public Guid MailAccountId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime FailedTime { get; set; }
    public int RetryCount { get; set; }
    public string? ExternalBizRef { get; set; }
}
