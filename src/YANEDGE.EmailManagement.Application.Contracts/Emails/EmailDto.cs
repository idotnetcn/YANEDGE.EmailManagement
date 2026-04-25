using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Emails;

public class EmailDto : FullAuditedEntityDto<Guid>
{
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromDisplayName { get; set; } = string.Empty;
    public EmailStatus Status { get; set; }
    public EmailPriority Priority { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? SentAt { get; set; }
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsBodyHtml { get; set; }
    public List<EmailRecipientDto> Recipients { get; set; } = new();
    public List<EmailAttachmentDto> Attachments { get; set; } = new();
}
