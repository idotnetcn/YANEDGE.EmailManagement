using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Accounts;

public class MailAccountAccessLog : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailAccountId { get; set; }
    public Guid? UserId { get; set; }
    public string AccessAction { get; set; } = null!;
    public byte AccessResult { get; set; }
    public byte? PermissionSource { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? TraceId { get; set; }
    public string? FailReason { get; set; }
    public DateTime CreationTime { get; set; }

    protected MailAccountAccessLog() { }

    public MailAccountAccessLog(Guid id, Guid mailAccountId, string accessAction, byte accessResult)
    {
        Id = id;
        MailAccountId = mailAccountId;
        AccessAction = accessAction;
        AccessResult = accessResult;
        CreationTime = DateTime.UtcNow;
    }
}
