using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace YANEDGE.EmailManagement.Conversations;

public class MailAttachment : AuditedEntity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailMessageId { get; private set; }
    public string FileName { get; set; } = null!;
    public string? FileExtension { get; set; }
    public string? ContentType { get; set; }
    public long SizeInBytes { get; set; }
    public string? FileHash { get; set; }
    public string? BlobContainer { get; set; }
    public string BlobName { get; set; } = null!;
    public byte StorageType { get; set; } = 1;
    public bool IsInline { get; set; }
    public string? ContentId { get; set; }
    public byte? PreviewStatus { get; set; }
    public byte? VirusScanStatus { get; set; }
    public byte AccessLevel { get; set; } = 1;
    public byte SecurityLevel { get; set; } = 1;
    public string? ExtraProperties { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? DeleterId { get; set; }
    public DateTime? DeletionTime { get; set; }

    protected MailAttachment() { }

    public MailAttachment(Guid id, Guid mailMessageId, string fileName, string blobName, long sizeInBytes)
    {
        Id = id;
        MailMessageId = mailMessageId;
        FileName = fileName;
        BlobName = blobName;
        SizeInBytes = sizeInBytes;
    }
}
