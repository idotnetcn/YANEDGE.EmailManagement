using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Accounts;

public class MailAccountSyncPolicy : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailAccountId { get; private set; }
    public bool SyncInbox { get; set; } = true;
    public bool SyncSent { get; set; } = true;
    public bool SyncDraft { get; set; }
    public bool SyncDeleted { get; set; }
    public bool SyncSpam { get; set; }
    public bool SyncArchive { get; set; }
    public bool SyncCustomFolders { get; set; } = true;
    public int MaxFetchCountPerRound { get; set; } = 200;
    public bool SyncBody { get; set; } = true;
    public bool SyncAttachments { get; set; } = true;
    public int? AttachmentSizeLimitMb { get; set; }
    public bool FullSyncOnFirstRun { get; set; } = true;
    public int? KeepDays { get; set; }
    public string? LastUidPerFolder { get; set; }
    public string? ExtraProperties { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? DeleterId { get; set; }
    public DateTime? DeletionTime { get; set; }

    protected MailAccountSyncPolicy() { }

    public MailAccountSyncPolicy(Guid id, Guid mailAccountId)
    {
        Id = id;
        MailAccountId = mailAccountId;
        CreationTime = DateTime.UtcNow;
    }
}
