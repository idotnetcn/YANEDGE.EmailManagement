using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.Template;

public class UpdateMailTemplateInput
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(50)]
    public string? Language { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(500)]
    public string SubjectTemplate { get; set; } = null!;

    [Required]
    public string BodyTemplate { get; set; } = null!;

    public string? PlainTextTemplate { get; set; }

    public bool RequiresApproval { get; set; }
}
