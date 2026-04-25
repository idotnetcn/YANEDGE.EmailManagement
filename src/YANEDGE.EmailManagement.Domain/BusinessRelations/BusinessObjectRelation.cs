using System;
using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.BusinessRelations;

public class BusinessObjectRelation : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid? MailMessageId { get; private set; }
    public Guid? ThreadId { get; private set; }
    public RelationType RelationType { get; set; }
    public string BusinessObjectType { get; set; } = null!;
    public string BusinessObjectId { get; set; } = null!;
    public string? BusinessObjectNo { get; set; }
    public string? BusinessObjectName { get; set; }
    public bool IsPrimary { get; set; }
    public PermissionSourceType SourceType { get; set; } = PermissionSourceType.Manual;
    public string? SourceSystem { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    protected BusinessObjectRelation() { }

    public static BusinessObjectRelation CreateForMessage(Guid id, Guid mailMessageId, RelationType relationType, string objectType, string objectId)
    {
        return new BusinessObjectRelation
        {
            Id = id,
            MailMessageId = mailMessageId,
            RelationType = relationType,
            BusinessObjectType = objectType,
            BusinessObjectId = objectId,
            CreationTime = DateTime.UtcNow
        };
    }

    public static BusinessObjectRelation CreateForThread(Guid id, Guid threadId, RelationType relationType, string objectType, string objectId)
    {
        return new BusinessObjectRelation
        {
            Id = id,
            ThreadId = threadId,
            RelationType = relationType,
            BusinessObjectType = objectType,
            BusinessObjectId = objectId,
            CreationTime = DateTime.UtcNow
        };
    }
}
