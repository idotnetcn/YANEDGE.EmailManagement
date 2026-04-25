using System;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Rules;

public class MailDomainPolicy : AuditedAggregateRoot<Guid>
{
    public Guid? TenantId { get; set; }
    public string DomainName { get; private set; } = null!;
    public DomainPolicyType PolicyType { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? Description { get; set; }

    protected MailDomainPolicy() { }

    public MailDomainPolicy(Guid id, string domainName, DomainPolicyType policyType)
    {
        Id = id;
        DomainName = domainName;
        PolicyType = policyType;
    }
}
