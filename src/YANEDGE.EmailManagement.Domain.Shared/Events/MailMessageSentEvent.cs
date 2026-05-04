using System;
using Volo.Abp.EventBus;

namespace YANEDGE.EmailManagement.Events;

/// <summary>
/// 邮件发送成功事件
/// Event triggered when a mail message is sent successfully
/// </summary>
[EventName("EmailManagement.MailMessage.Sent")]
public class MailMessageSentEvent : EtoBase
{
    public Guid SendTaskId { get; set; }
    public Guid? MessageId { get; set; }
    public Guid MailAccountId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string ToAddresses { get; set; } = string.Empty;
    public DateTime SentTime { get; set; }
    public bool HasAttachments { get; set; }
    public string? ExternalBizRef { get; set; }
}
