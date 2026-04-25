using System;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.BusinessRelations;

public class Contact : FullAuditedAggregateRoot<Guid>
{
    public Guid? TenantId { get; set; }
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
    public bool IsEnabled { get; set; } = true;
    public string? ExtraProperties { get; set; }

    protected Contact() { }

    public Contact(Guid id, string name, string emailAddress, ContactType contactType)
    {
        Id = id;
        Name = name;
        EmailAddress = emailAddress;
        ContactType = contactType;
    }
}
