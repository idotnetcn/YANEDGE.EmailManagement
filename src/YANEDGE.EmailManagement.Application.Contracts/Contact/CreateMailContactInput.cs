using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.Contact;

public class CreateMailContactInput
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(200)]
    [EmailAddress]
    public string EmailAddress { get; set; } = null!;

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [StringLength(200)]
    public string? CompanyName { get; set; }

    [StringLength(100)]
    public string? JobTitle { get; set; }

    [StringLength(100)]
    public string? CustomerId { get; set; }

    [StringLength(100)]
    public string? SupplierId { get; set; }

    [StringLength(50)]
    public string Source { get; set; } = "Manual";

    [StringLength(100)]
    public string? ExternalId { get; set; }

    [StringLength(2000)]
    public string? Notes { get; set; }
}
