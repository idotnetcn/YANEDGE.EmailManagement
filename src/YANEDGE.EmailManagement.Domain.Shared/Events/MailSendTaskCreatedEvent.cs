using System;
using Volo.Abp.EventBus;

namespace YANEDGE.EmailManagement.Events;

/// <summary>
/// 发送任务创建事件
/// Event triggered when a mail send task is created
/// </summary>
[EventName("EmailManagement.MailSendTask.Created")]
public class MailSendTaskCreatedEvent : EtoBase
{
    public Guid SendTaskId { get; set; }
    public Guid MailAccountId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string ToAddresses { get; set; } = string.Empty;
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
    public bool RequiresApproval { get; set; }
    public string? ExternalBizRef { get; set; }
}
