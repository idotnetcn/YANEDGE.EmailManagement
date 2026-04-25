using System;
using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Accounts;

public class MailAccountHealthLog : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailAccountId { get; set; }
    public byte CheckType { get; set; }
    public HealthStatus HealthStatus { get; set; }
    public string? ServerHost { get; set; }
    public int? ServerPort { get; set; }
    public int? ResponseTimeMs { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? TraceId { get; set; }
    public Guid? CheckedByUserId { get; set; }
    public DateTime CreationTime { get; set; }

    protected MailAccountHealthLog() { }

    public MailAccountHealthLog(Guid id, Guid mailAccountId)
    {
        Id = id;
        MailAccountId = mailAccountId;
        CreationTime = DateTime.UtcNow;
    }
}
