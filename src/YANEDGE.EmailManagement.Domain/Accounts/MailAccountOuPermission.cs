using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Accounts;

public class MailAccountOuPermission : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailAccountId { get; private set; }
    public Guid OrganizationUnitId { get; private set; }
    public bool CanView { get; set; } = true;
    public bool CanSend { get; set; }
    public bool CanManage { get; set; }
    public bool CanAssign { get; set; }
    public bool CanDownloadAttachment { get; set; } = true;
    public bool CanViewSensitive { get; set; }
    public bool InheritToChildren { get; set; } = true;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }

    protected MailAccountOuPermission() { }

    public MailAccountOuPermission(Guid id, Guid mailAccountId, Guid organizationUnitId)
    {
        Id = id;
        MailAccountId = mailAccountId;
        OrganizationUnitId = organizationUnitId;
        CreationTime = DateTime.UtcNow;
    }
}
