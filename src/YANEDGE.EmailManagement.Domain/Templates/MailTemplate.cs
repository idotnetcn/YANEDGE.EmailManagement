using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Templates;

public class MailTemplate : FullAuditedAggregateRoot<Guid>
{
    public Guid? TenantId { get; set; }
    public string Code { get; private set; } = null!;
    public string Name { get; set; } = null!;
    public Guid? CategoryId { get; set; }
    public string? SubjectTemplate { get; set; }
    public BodyFormat BodyFormat { get; set; }
    public string? LanguageCode { get; set; }
    public int CurrentVersionNo { get; private set; } = 1;
    public bool IsEnabled { get; set; } = true;
    public bool NeedApproval { get; set; }
    public string? Description { get; set; }
    public string? ExtraProperties { get; set; }

    public virtual ICollection<MailTemplateVersion> Versions { get; private set; } = new List<MailTemplateVersion>();

    protected MailTemplate() { }

    public MailTemplate(Guid id, string code, string name, BodyFormat bodyFormat)
    {
        Id = id;
        Code = code;
        Name = name;
        BodyFormat = bodyFormat;
    }

    public void IncrementVersion() => CurrentVersionNo++;
}
