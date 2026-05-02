using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.Label;

public class UpdateMailLabelInput
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string Color { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }
}
