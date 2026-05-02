using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.BusinessRelation;

public class UpdateMailBusinessRelationInput
{
    [StringLength(200)]
    public string? BusinessObjectName { get; set; }

    [StringLength(100)]
    public string? BusinessObjectCode { get; set; }

    public bool? IsPrimary { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
