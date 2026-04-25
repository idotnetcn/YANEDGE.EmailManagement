using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Integrations;

public class ExternalMapping : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public string SourceSystem { get; set; } = null!;
    public string ExternalId { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public Guid InternalId { get; set; }
    public DateTime CreationTime { get; set; }

    protected ExternalMapping() { }

    public ExternalMapping(Guid id, string sourceSystem, string externalId, string entityType, Guid internalId)
    {
        Id = id;
        SourceSystem = sourceSystem;
        ExternalId = externalId;
        EntityType = entityType;
        InternalId = internalId;
        CreationTime = DateTime.UtcNow;
    }
}
