using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Accounts;

public class MailAccount : FullAuditedAggregateRoot<Guid>
{
    public Guid? TenantId { get; private set; }
    public Guid? OrganizationUnitId { get; set; }
    public string Name { get; private set; } = null!;
    public string EmailAddress { get; private set; } = null!;
    public string? DisplayName { get; set; }
    public AccountType AccountType { get; private set; }
    public OwnerType? OwnerType { get; private set; }
    public Guid? OwnerUserId { get; private set; }
    public Guid? OwnerRoleId { get; private set; }
    public Guid? OwnerOrganizationUnitId { get; private set; }
    public bool IsShared { get; private set; }
    public VisibilityScope VisibilityScope { get; private set; }
    public bool AllowDelegateSend { get; set; }
    public bool AllowDelegateManage { get; set; }
    public bool RequiresExplicitAuthorization { get; set; }
    public InboundProtocol InboundProtocol { get; private set; }
    public OutboundProtocol OutboundProtocol { get; private set; }
    public string? ImapHost { get; set; }
    public int? ImapPort { get; set; }
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }
    public bool UseSsl { get; set; }
    public bool SyncEnabled { get; private set; }
    public bool SendEnabled { get; private set; }
    public int SyncIntervalSeconds { get; set; }
    public int MaxParallelSyncCount { get; set; }
    public int? DailySendLimit { get; set; }
    public DateTime? LastSyncTime { get; set; }
    public SyncStatus? LastSyncStatus { get; set; }
    public HealthStatus? HealthStatus { get; set; }
    public AccountStatus Status { get; private set; }
    public string? Description { get; set; }
    public string? ExtraProperties { get; set; }

    public virtual ICollection<MailAccountCredential> Credentials { get; private set; } = new List<MailAccountCredential>();
    public virtual ICollection<MailAccountSyncPolicy> SyncPolicies { get; private set; } = new List<MailAccountSyncPolicy>();
    public virtual ICollection<MailAccountUserPermission> UserPermissions { get; private set; } = new List<MailAccountUserPermission>();
    public virtual ICollection<MailAccountRolePermission> RolePermissions { get; private set; } = new List<MailAccountRolePermission>();
    public virtual ICollection<MailAccountOuPermission> OuPermissions { get; private set; } = new List<MailAccountOuPermission>();

    protected MailAccount() { }

    public MailAccount(
        Guid id,
        Guid? tenantId,
        string name,
        string emailAddress,
        AccountType accountType,
        InboundProtocol inboundProtocol = InboundProtocol.IMAP,
        OutboundProtocol outboundProtocol = OutboundProtocol.SMTP)
    {
        Id = id;
        TenantId = tenantId;
        Name = name;
        EmailAddress = emailAddress;
        AccountType = accountType;
        InboundProtocol = inboundProtocol;
        OutboundProtocol = outboundProtocol;
        SyncEnabled = true;
        SendEnabled = true;
        SyncIntervalSeconds = 300;
        MaxParallelSyncCount = 1;
        Status = AccountStatus.Active;
        VisibilityScope = VisibilityScope.ExplicitOnly;
        RequiresExplicitAuthorization = true;
        UseSsl = true;
    }

    public void EnableSync() => SyncEnabled = true;
    public void DisableSync() => SyncEnabled = false;
    public void EnableSend() => SendEnabled = true;
    public void DisableSend() => SendEnabled = false;
    public void Activate() => Status = AccountStatus.Active;
    public void Deactivate() => Status = AccountStatus.Disabled;

    public void ChangeOwner(OwnerType ownerType, Guid? userId, Guid? roleId, Guid? ouId)
    {
        OwnerType = ownerType;
        OwnerUserId = userId;
        OwnerRoleId = roleId;
        OwnerOrganizationUnitId = ouId;
    }

    public void ConfigureInbound(string host, int port, InboundProtocol protocol)
    {
        ImapHost = host;
        ImapPort = port;
        InboundProtocol = protocol;
    }

    public void ConfigureOutbound(string host, int port, OutboundProtocol protocol)
    {
        SmtpHost = host;
        SmtpPort = port;
        OutboundProtocol = protocol;
    }
}
