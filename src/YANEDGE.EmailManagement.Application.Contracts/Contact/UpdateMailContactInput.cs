using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.Contact;

public class UpdateMailContactInput
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [StringLength(200)]
    public string? CompanyName { get; set; }

    [StringLength(100)]
    public string? JobTitle { get; set; }

    [StringLength(2000)]
    public string? Notes { get; set; }
}
