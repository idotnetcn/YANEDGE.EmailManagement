using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Emails;

public class EmailAttachment : Entity<Guid>
{
    public Guid EmailId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long FileSize { get; private set; }
    public string StoragePath { get; private set; } = null!;

    protected EmailAttachment() { }

    public EmailAttachment(Guid id, Guid emailId, string fileName, string contentType, long fileSize, string storagePath)
        : base(id)
    {
        EmailId = emailId;
        FileName = Check.NotNullOrWhiteSpace(fileName, nameof(fileName));
        ContentType = Check.NotNullOrWhiteSpace(contentType, nameof(contentType));
        FileSize = fileSize;
        StoragePath = Check.NotNullOrWhiteSpace(storagePath, nameof(storagePath));
    }
}
