using System;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Conversations;

public class MailFolder : FullAuditedAggregateRoot<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailAccountId { get; private set; }
    public Guid? ParentId { get; set; }
    public FolderType FolderType { get; set; } = FolderType.Custom;
    public string FolderName { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? ExternalFolderId { get; set; }
    public string? ExternalPath { get; set; }
    public string? Delimiter { get; set; }
    public bool IsSystem { get; set; }
    public bool IsSelectable { get; set; } = true;
    public bool IsSyncEnabled { get; set; } = true;
    public int SortOrder { get; set; }
    public int MessageCount { get; set; }
    public int UnreadCount { get; set; }
    public DateTime? LastSyncTime { get; set; }

    protected MailFolder() { }

    public MailFolder(Guid id, Guid mailAccountId, string folderName, FolderType folderType = FolderType.Custom)
    {
        Id = id;
        MailAccountId = mailAccountId;
        FolderName = folderName;
        FolderType = folderType;
    }
}
