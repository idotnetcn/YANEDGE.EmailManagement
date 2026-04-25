using System.ComponentModel.DataAnnotations;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Contacts;

public class CreateUpdateContactInput
{
    public ContactType ContactType { get; set; } = ContactType.Other;

    [Required, MaxLength(128)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(256)]
    public string EmailAddress { get; set; } = null!;

    [MaxLength(64)]
    public string? Mobile { get; set; }

    [MaxLength(256)]
    public string? CompanyName { get; set; }

    [MaxLength(128)]
    public string? ExternalId { get; set; }

    [MaxLength(64)]
    public string? SourceSystem { get; set; }

    [MaxLength(128)]
    public string? DepartmentName { get; set; }

    [MaxLength(128)]
    public string? Title { get; set; }

    [MaxLength(512)]
    public string? Remark { get; set; }
}
