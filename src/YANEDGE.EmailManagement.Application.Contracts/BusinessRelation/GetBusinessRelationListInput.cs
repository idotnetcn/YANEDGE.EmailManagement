using System;
using Volo.Abp.Application.Dtos;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.BusinessRelation;

public class GetBusinessRelationListInput : PagedAndSortedResultRequestDto
{
    public Guid? MailMessageId { get; set; }

    public Guid? ThreadId { get; set; }

    public BusinessObjectType? BusinessObjectType { get; set; }

    public string? BusinessObjectId { get; set; }

    public string? RelationSource { get; set; }

    public bool? IsPrimary { get; set; }
}
