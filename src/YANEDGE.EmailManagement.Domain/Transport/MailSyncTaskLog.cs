using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Transport;

public class MailSyncTaskLog : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailSyncTaskId { get; private set; }
    public string PhaseName { get; set; } = null!;
    public string? FolderPath { get; set; }
    public byte LogLevel { get; set; } = 1;
    public string Message { get; set; } = null!;
    public int? FetchedCount { get; set; }
    public int? InsertedCount { get; set; }
    public int? UpdatedCount { get; set; }
    public int? SkippedCount { get; set; }
    public int? FailedCount { get; set; }
    public string? ExternalUid { get; set; }
    public DateTime CreationTime { get; set; }

    protected MailSyncTaskLog() { }

    public MailSyncTaskLog(Guid id, Guid mailSyncTaskId, string phaseName, string message)
    {
        Id = id;
        MailSyncTaskId = mailSyncTaskId;
        PhaseName = phaseName;
        Message = message;
        CreationTime = DateTime.UtcNow;
    }
}
