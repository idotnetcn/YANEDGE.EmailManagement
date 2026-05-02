using System;
using Volo.Abp.Application.Dtos;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.BusinessRelation;

public class MailBusinessRelationDto : CreationAuditedEntityDto<Guid>
{
    public Guid MailMessageId { get; set; }

    public Guid? ThreadId { get; set; }

    public BusinessObjectType BusinessObjectType { get; set; }

    public string BusinessObjectId { get; set; } = null!;

    public string? BusinessObjectName { get; set; }

    public string? BusinessObjectCode { get; set; }

    public bool IsPrimary { get; set; }

    public string RelationSource { get; set; } = null!;

    public string? ExternalSystem { get; set; }

    public string? Notes { get; set; }
}
