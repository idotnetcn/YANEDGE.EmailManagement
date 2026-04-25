using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Integrations;

public class IdempotencyRecord : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public string IdempotencyKey { get; private set; } = null!;
    public string OperationType { get; set; } = null!;
    public string? ResultJson { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime ExpiresAt { get; set; }

    protected IdempotencyRecord() { }

    public IdempotencyRecord(Guid id, string idempotencyKey, string operationType, DateTime expiresAt)
    {
        Id = id;
        IdempotencyKey = idempotencyKey;
        OperationType = operationType;
        ExpiresAt = expiresAt;
        CreationTime = DateTime.UtcNow;
    }
}
