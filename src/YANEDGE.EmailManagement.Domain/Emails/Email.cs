using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace YANEDGE.EmailManagement.Emails;

public class Email : FullAuditedAggregateRoot<Guid>
{
    public string Subject { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public string FromAddress { get; private set; } = null!;
    public string FromDisplayName { get; private set; } = string.Empty;
    public EmailStatus Status { get; private set; }
    public EmailPriority Priority { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public int RetryCount { get; private set; }
    public string? ErrorMessage { get; private set; }
    public bool IsBodyHtml { get; private set; }
    public ICollection<EmailRecipient> Recipients { get; private set; } = new List<EmailRecipient>();
    public ICollection<EmailAttachment> Attachments { get; private set; } = new List<EmailAttachment>();

    protected Email() { }

    public Email(
        Guid id,
        string subject,
        string body,
        string fromAddress,
        bool isBodyHtml = true,
        EmailPriority priority = EmailPriority.Normal,
        string fromDisplayName = "")
        : base(id)
    {
        Subject = Check.NotNullOrWhiteSpace(subject, nameof(subject), EmailConsts.MaxSubjectLength);
        Body = Check.NotNullOrWhiteSpace(body, nameof(body));
        FromAddress = Check.NotNullOrWhiteSpace(fromAddress, nameof(fromAddress), EmailConsts.MaxAddressLength);
        FromDisplayName = fromDisplayName;
        IsBodyHtml = isBodyHtml;
        Priority = priority;
        Status = EmailStatus.Draft;
        RetryCount = 0;
    }

    public void Queue(DateTime? scheduledAt = null)
    {
        Status = EmailStatus.Queued;
        ScheduledAt = scheduledAt;
    }

    public void MarkAsSending()
    {
        Status = EmailStatus.Sending;
    }

    public void MarkAsSent()
    {
        Status = EmailStatus.Sent;
        SentAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = EmailStatus.Failed;
        ErrorMessage = errorMessage;
        RetryCount++;
    }

    public void Cancel()
    {
        Status = EmailStatus.Cancelled;
    }

    public void UpdateContent(string subject, string body)
    {
        Subject = Check.NotNullOrWhiteSpace(subject, nameof(subject), EmailConsts.MaxSubjectLength);
        Body = Check.NotNullOrWhiteSpace(body, nameof(body));
    }
}
