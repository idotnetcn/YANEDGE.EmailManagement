using System;
using Volo.Abp.EventBus;

namespace YANEDGE.EmailManagement.Events;

/// <summary>
/// 邮件接收事件
/// Event triggered when a new mail message is received
/// </summary>
[EventName("EmailManagement.MailMessage.Received")]
public class MailMessageReceivedEvent : EtoBase
{
    public Guid MessageId { get; set; }
    public Guid ThreadId { get; set; }
    public Guid MailAccountId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string? ToAddresses { get; set; }
    public DateTime ReceivedTime { get; set; }
    public bool HasAttachments { get; set; }
    public int AttachmentCount { get; set; }
}
