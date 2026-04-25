using System;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Tags;

public class MailTag : FullAuditedAggregateRoot<Guid>
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
    public TagType TagType { get; set; }
    public int SortOrder { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? Description { get; set; }

    protected MailTag() { }

    public MailTag(Guid id, string name, TagType tagType)
    {
        Id = id;
        Name = name;
        TagType = tagType;
    }
}
