using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Conversations;

public class MailMessage : FullAuditedAggregateRoot<Guid>
{
    public Guid? TenantId { get; private set; }
    public Guid MailAccountId { get; private set; }
    public Guid? ThreadId { get; private set; }
    public FolderType FolderType { get; set; }
    public string? InternetMessageId { get; set; }
    public string? InReplyTo { get; set; }
    public string? ReferencesText { get; set; }
    public string? Subject { get; set; }
    public string? NormalizedSubject { get; set; }
    public string? FromAddress { get; set; }
    public string? FromName { get; set; }
    public string? SenderAddress { get; set; }
    public string? ReplyTo { get; set; }
    public BodyFormat BodyFormat { get; set; } = BodyFormat.Html;
    public string? BodyPreview { get; set; }
    public DateTime? SentTime { get; set; }
    public DateTime? ReceivedTime { get; set; }
    public MailDirection MailDirection { get; private set; }
    public ProcessingStatus ProcessingStatus { get; set; } = ProcessingStatus.PendingAssignment;
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.NotRequired;
    public Importance Importance { get; set; } = Importance.Normal;
    public bool HasAttachments { get; set; }
    public long SizeInBytes { get; set; }
    public string? ExternalMessageUid { get; set; }
    public string? RawHeaders { get; set; }
    public bool IsLocked { get; set; }
    public Guid? LockUserId { get; set; }
    public DateTime? LockTime { get; set; }
    public string? BusinessStatus { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public Guid? SubmittedByUserId { get; set; }
    public Guid? SentByUserId { get; set; }
    public Guid? LastOperatorUserId { get; set; }
    public Guid? SourceUserId { get; set; }
    public SecurityLevel SecurityLevel { get; set; } = SecurityLevel.Normal;
    public VisibilityPolicy VisibilityPolicy { get; set; } = VisibilityPolicy.InheritMailbox;
    public string? ExtraProperties { get; set; }

    public virtual MailMessageBody? Body { get; set; }
    public virtual ICollection<MailRecipient> Recipients { get; private set; } = new List<MailRecipient>();
    public virtual ICollection<MailMessageHeader> Headers { get; private set; } = new List<MailMessageHeader>();
    public virtual ICollection<MailAttachment> Attachments { get; private set; } = new List<MailAttachment>();

    protected MailMessage() { }

    public MailMessage(Guid id, Guid? tenantId, Guid mailAccountId, MailDirection direction, FolderType folderType)
    {
        Id = id;
        TenantId = tenantId;
        MailAccountId = mailAccountId;
        MailDirection = direction;
        FolderType = folderType;
    }

    public void AssignToThread(Guid threadId) => ThreadId = threadId;
}
