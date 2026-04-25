using System;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Templates;

public class MailSignature : FullAuditedAggregateRoot<Guid>
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; } = null!;
    public OwnerType OwnerType { get; set; }
    public Guid? OwnerId { get; set; }
    public BodyFormat BodyFormat { get; set; }
    public string Content { get; set; } = null!;
    public bool IsDefault { get; set; }
    public bool IsEnabled { get; set; } = true;

    protected MailSignature() { }

    public MailSignature(Guid id, string name, OwnerType ownerType, BodyFormat bodyFormat, string content)
    {
        Id = id;
        Name = name;
        OwnerType = ownerType;
        BodyFormat = bodyFormat;
        Content = content;
    }
}
