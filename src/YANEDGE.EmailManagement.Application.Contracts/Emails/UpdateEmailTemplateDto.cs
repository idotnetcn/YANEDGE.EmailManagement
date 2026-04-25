using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.Emails;

public class UpdateEmailTemplateDto
{
    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    public bool IsBodyHtml { get; set; } = true;
    public string? Description { get; set; }
}
