using System;
using Volo.Abp.EventBus;

namespace YANEDGE.EmailManagement.Domain.Shared.Events;

/// <summary>
/// 附件访问事件
/// Event triggered when attachment is accessed/downloaded
/// </summary>
[EventName("EmailManagement.MailAttachment.Accessed")]
public class MailAttachmentAccessedEvent 
{
    public Guid AttachmentId { get; set; }
    public Guid MessageId { get; set; }
    public Guid ThreadId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Guid? AccessedByUserId { get; set; }
    public DateTime AccessedTime { get; set; }
    public string AccessType { get; set; } = "Download"; // Download, Preview, etc.
}
