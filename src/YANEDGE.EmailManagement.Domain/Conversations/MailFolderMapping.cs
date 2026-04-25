using System;
using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Conversations;

public class MailFolderMapping : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailAccountId { get; private set; }
    public FolderType FolderType { get; private set; }
    public Guid MailFolderId { get; private set; }
    public bool IsPrimary { get; set; } = true;
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }

    protected MailFolderMapping() { }

    public MailFolderMapping(Guid id, Guid mailAccountId, FolderType folderType, Guid mailFolderId)
    {
        Id = id;
        MailAccountId = mailAccountId;
        FolderType = folderType;
        MailFolderId = mailFolderId;
        CreationTime = DateTime.UtcNow;
    }
}
