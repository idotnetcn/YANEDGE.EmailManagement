using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Auditing;

public class MailAuditLog : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public Guid? EntityId { get; set; }
    public string ActionName { get; set; } = null!;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? TraceId { get; set; }
    public DateTime CreationTime { get; set; }

    protected MailAuditLog() { }

    public MailAuditLog(Guid id, string entityType, string actionName, string userName)
    {
        Id = id;
        EntityType = entityType;
        ActionName = actionName;
        UserName = userName;
        CreationTime = DateTime.UtcNow;
    }
}
