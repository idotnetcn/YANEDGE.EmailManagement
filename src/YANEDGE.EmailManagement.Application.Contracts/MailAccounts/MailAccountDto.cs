using System;
using YANEDGE.EmailManagement.Enums;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.MailAccounts;

public class MailAccountDto : FullAuditedEntityDto<Guid>
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
    public string? DisplayName { get; set; }
    public AccountType AccountType { get; set; }
    public OwnerType? OwnerType { get; set; }
    public Guid? OwnerUserId { get; set; }
    public bool IsShared { get; set; }
    public VisibilityScope VisibilityScope { get; set; }
    public InboundProtocol InboundProtocol { get; set; }
    public OutboundProtocol OutboundProtocol { get; set; }
    public string? ImapHost { get; set; }
    public int? ImapPort { get; set; }
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }
    public bool UseSsl { get; set; }
    public bool SyncEnabled { get; set; }
    public bool SendEnabled { get; set; }
    public int SyncIntervalSeconds { get; set; }
    public AccountStatus Status { get; set; }
    public HealthStatus? HealthStatus { get; set; }
    public DateTime? LastSyncTime { get; set; }
    public string? Description { get; set; }
}
