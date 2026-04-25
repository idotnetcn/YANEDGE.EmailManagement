using System;
using YANEDGE.EmailManagement.Enums;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Contacts;

public class ContactDto : FullAuditedEntityDto<Guid>
{
    public ContactType ContactType { get; set; }
    public string Name { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
    public string? Mobile { get; set; }
    public string? CompanyName { get; set; }
    public string? ExternalId { get; set; }
    public string? SourceSystem { get; set; }
    public string? DepartmentName { get; set; }
    public string? Title { get; set; }
    public string? Remark { get; set; }
    public bool IsEnabled { get; set; }
}
