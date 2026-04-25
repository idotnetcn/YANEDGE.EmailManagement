using System;
using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Accounts;

public class MailAccountUserPermission : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailAccountId { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsOwner { get; set; }
    public bool CanView { get; set; } = true;
    public bool CanSend { get; set; }
    public bool CanManage { get; set; }
    public bool CanAssign { get; set; }
    public bool CanDownloadAttachment { get; set; } = true;
    public bool CanViewSensitive { get; set; }
    public bool CanDeleteMail { get; set; }
    public bool CanArchiveMail { get; set; } = true;
    public int AccessPriority { get; set; } = 100;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public PermissionSourceType SourceType { get; set; } = PermissionSourceType.Manual;
    public string? Remark { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }

    protected MailAccountUserPermission() { }

    public MailAccountUserPermission(Guid id, Guid mailAccountId, Guid userId)
    {
        Id = id;
        MailAccountId = mailAccountId;
        UserId = userId;
        CreationTime = DateTime.UtcNow;
    }
}
