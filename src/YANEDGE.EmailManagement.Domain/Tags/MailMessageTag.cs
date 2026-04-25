using System;
using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Tags;

public class MailMessageTag : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailMessageId { get; private set; }
    public Guid MailTagId { get; private set; }
    public PermissionSourceType SourceType { get; set; } = PermissionSourceType.Manual;
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }

    protected MailMessageTag() { }

    public MailMessageTag(Guid id, Guid mailMessageId, Guid mailTagId)
    {
        Id = id;
        MailMessageId = mailMessageId;
        MailTagId = mailTagId;
        CreationTime = DateTime.UtcNow;
    }
}
