using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Accounts;

public class MailAccountRolePermission : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailAccountId { get; private set; }
    public Guid RoleId { get; private set; }
    public bool CanView { get; set; } = true;
    public bool CanSend { get; set; }
    public bool CanManage { get; set; }
    public bool CanAssign { get; set; }
    public bool CanDownloadAttachment { get; set; } = true;
    public bool CanViewSensitive { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }

    protected MailAccountRolePermission() { }

    public MailAccountRolePermission(Guid id, Guid mailAccountId, Guid roleId)
    {
        Id = id;
        MailAccountId = mailAccountId;
        RoleId = roleId;
        CreationTime = DateTime.UtcNow;
    }
}
