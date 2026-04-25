using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace YANEDGE.EmailManagement.Emails;

public class EmailTemplate : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string Subject { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public bool IsBodyHtml { get; private set; }
    public bool IsActive { get; private set; }
    public string? Description { get; private set; }

    protected EmailTemplate() { }

    public EmailTemplate(Guid id, string name, string subject, string body, bool isBodyHtml = true, string? description = null)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), EmailConsts.MaxTemplateNameLength);
        Subject = Check.NotNullOrWhiteSpace(subject, nameof(subject), EmailConsts.MaxSubjectLength);
        Body = Check.NotNullOrWhiteSpace(body, nameof(body));
        IsBodyHtml = isBodyHtml;
        IsActive = true;
        Description = description;
    }

    public void Update(string name, string subject, string body, bool isBodyHtml, string? description)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), EmailConsts.MaxTemplateNameLength);
        Subject = Check.NotNullOrWhiteSpace(subject, nameof(subject), EmailConsts.MaxSubjectLength);
        Body = Check.NotNullOrWhiteSpace(body, nameof(body));
        IsBodyHtml = isBodyHtml;
        Description = description;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
