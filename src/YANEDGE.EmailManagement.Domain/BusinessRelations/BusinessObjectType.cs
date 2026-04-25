using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.BusinessRelations;

public class BusinessObjectType : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public string Code { get; private set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsEnabled { get; set; } = true;
    public string? SourceSystem { get; set; }
    public string? Description { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }

    protected BusinessObjectType() { }

    public BusinessObjectType(Guid id, string code, string name)
    {
        Id = id;
        Code = code;
        Name = name;
        CreationTime = DateTime.UtcNow;
    }
}
