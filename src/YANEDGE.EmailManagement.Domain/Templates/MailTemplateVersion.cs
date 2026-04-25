using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Templates;

public class MailTemplateVersion : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailTemplateId { get; private set; }
    public int VersionNo { get; private set; }
    public string? SubjectTemplate { get; set; }
    public string? BodyContent { get; set; }
    public string? ChangeSummary { get; set; }
    public bool IsPublished { get; set; }
    public Guid? PublisherId { get; set; }
    public DateTime? PublishTime { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }

    protected MailTemplateVersion() { }

    public MailTemplateVersion(Guid id, Guid mailTemplateId, int versionNo)
    {
        Id = id;
        MailTemplateId = mailTemplateId;
        VersionNo = versionNo;
        CreationTime = DateTime.UtcNow;
    }

    public void Publish(Guid publisherId)
    {
        IsPublished = true;
        PublisherId = publisherId;
        PublishTime = DateTime.UtcNow;
    }
}
