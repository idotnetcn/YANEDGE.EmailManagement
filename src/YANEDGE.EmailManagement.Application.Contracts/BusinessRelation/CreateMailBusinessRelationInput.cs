using System;
using System.ComponentModel.DataAnnotations;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.BusinessRelation;

public class CreateMailBusinessRelationInput
{
    [Required]
    public Guid MailMessageId { get; set; }

    public Guid? ThreadId { get; set; }

    [Required]
    public BusinessObjectType BusinessObjectType { get; set; }

    [Required]
    [StringLength(100)]
    public string BusinessObjectId { get; set; } = null!;

    [StringLength(200)]
    public string? BusinessObjectName { get; set; }

    [StringLength(100)]
    public string? BusinessObjectCode { get; set; }

    public bool IsPrimary { get; set; }

    [StringLength(50)]
    public string RelationSource { get; set; } = "Manual";

    [StringLength(100)]
    public string? ExternalSystem { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
