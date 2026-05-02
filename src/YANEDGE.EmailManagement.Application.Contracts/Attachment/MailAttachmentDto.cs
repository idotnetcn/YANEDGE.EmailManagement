using System;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Attachment;

public class MailAttachmentDto : FullAuditedEntityDto<Guid>
{
    public Guid MailMessageId { get; set; }

    public string FileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long FileSize { get; set; }

    public string StoragePath { get; set; } = null!;

    public string? FileHash { get; set; }

    public string? ContentId { get; set; }

    public bool IsInline { get; set; }

    public bool IsSensitive { get; set; }

    public int DownloadCount { get; set; }

    public DateTime? LastDownloadedAt { get; set; }

    public bool IsScanned { get; set; }

    public bool IsSafe { get; set; }

    public string? ScanResult { get; set; }
}
