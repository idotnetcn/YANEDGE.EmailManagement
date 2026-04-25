using System;
using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Transport;

public class MailSyncTask : AggregateRoot<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailAccountId { get; private set; }
    public FolderType? FolderType { get; set; }
    public SyncStatus SyncStatus { get; private set; }
    public DateTime? SyncStartTime { get; private set; }
    public DateTime? SyncEndTime { get; private set; }
    public int FetchedCount { get; private set; }
    public int InsertedCount { get; private set; }
    public int UpdatedCount { get; private set; }
    public int SkippedCount { get; private set; }
    public int FailedCount { get; private set; }
    public string? LastUid { get; set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }

    protected MailSyncTask() { }

    public MailSyncTask(Guid id, Guid mailAccountId)
    {
        Id = id;
        MailAccountId = mailAccountId;
        SyncStatus = SyncStatus.Pending;
        CreationTime = DateTime.UtcNow;
    }

    public void Start()
    {
        SyncStatus = SyncStatus.Syncing;
        SyncStartTime = DateTime.UtcNow;
    }

    public void Complete(int fetched, int inserted, int updated, int skipped, int failed)
    {
        SyncEndTime = DateTime.UtcNow;
        FetchedCount = fetched;
        InsertedCount = inserted;
        UpdatedCount = updated;
        SkippedCount = skipped;
        FailedCount = failed;
        SyncStatus = failed > 0 && inserted == 0 ? SyncStatus.Failed :
                     failed > 0 ? SyncStatus.PartialSuccess : SyncStatus.Succeeded;
    }

    public void Fail(string errorMessage)
    {
        SyncStatus = SyncStatus.Failed;
        ErrorMessage = errorMessage;
        SyncEndTime = DateTime.UtcNow;
    }
}
