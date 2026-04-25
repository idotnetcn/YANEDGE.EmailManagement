using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace YANEDGE.EmailManagement.Templates;

public class MailTemplateCategory : AuditedAggregateRoot<Guid>
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; } = null!;
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }
    public bool IsEnabled { get; set; } = true;

    protected MailTemplateCategory() { }

    public MailTemplateCategory(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
