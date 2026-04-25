using System;
using System.Collections.Generic;
using YANEDGE.EmailManagement.Enums;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.MailMessages;

public class MailMessageDto : FullAuditedEntityDto<Guid>
{
    public Guid MailAccountId { get; set; }
    public Guid? ThreadId { get; set; }
    public FolderType FolderType { get; set; }
    public string? Subject { get; set; }
    public string? FromAddress { get; set; }
    public string? FromName { get; set; }
    public BodyFormat BodyFormat { get; set; }
    public string? BodyPreview { get; set; }
    public DateTime? SentTime { get; set; }
    public DateTime? ReceivedTime { get; set; }
    public MailDirection MailDirection { get; set; }
    public ProcessingStatus ProcessingStatus { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; }
    public Importance Importance { get; set; }
    public bool HasAttachments { get; set; }
    public long SizeInBytes { get; set; }
    public SecurityLevel SecurityLevel { get; set; }
    public VisibilityPolicy VisibilityPolicy { get; set; }
    public List<MailRecipientDto> Recipients { get; set; } = new();
}
