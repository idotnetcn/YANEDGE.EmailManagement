using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.Emails;

public class CreateEmailRecipientDto
{
    [Required]
    [MaxLength(256)]
    [EmailAddress]
    public string Address { get; set; } = string.Empty;

    [MaxLength(256)]
    public string DisplayName { get; set; } = string.Empty;

    public RecipientType RecipientType { get; set; } = RecipientType.To;
}
