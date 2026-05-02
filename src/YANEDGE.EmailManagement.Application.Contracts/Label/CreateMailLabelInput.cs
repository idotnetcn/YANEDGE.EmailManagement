using System;
using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.Label;

public class CreateMailLabelInput
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string Color { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    public Guid? OwnerUserId { get; set; }

    public Guid? OwnerOrganizationId { get; set; }

    public bool IsSystemLabel { get; set; }
}
