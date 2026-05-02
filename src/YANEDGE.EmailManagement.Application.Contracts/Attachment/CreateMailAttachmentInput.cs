using System;
using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.Attachment;

public class CreateMailAttachmentInput
{
    [Required]
    public Guid MailMessageId { get; set; }

    [Required]
    [StringLength(500)]
    public string FileName { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string ContentType { get; set; } = null!;

    [Required]
    public long FileSize { get; set; }

    [Required]
    [StringLength(1000)]
    public string StoragePath { get; set; } = null!;

    [StringLength(100)]
    public string? FileHash { get; set; }

    [StringLength(100)]
    public string? ContentId { get; set; }

    public bool IsInline { get; set; }

    public bool IsSensitive { get; set; }
}
