using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.Emails;

public class UpdateEmailDto
{
    [Required]
    [MaxLength(500)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;
}
