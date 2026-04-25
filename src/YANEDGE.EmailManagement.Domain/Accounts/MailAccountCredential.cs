using System;
using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Accounts;

public class MailAccountCredential : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailAccountId { get; private set; }
    public AuthType AuthType { get; set; }
    public string? UserName { get; set; }
    public string? EncryptedPassword { get; set; }
    public string? EncryptedAccessToken { get; set; }
    public string? EncryptedRefreshToken { get; set; }
    public DateTime? TokenExpireTime { get; set; }
    public string? ExtraProperties { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }

    protected MailAccountCredential() { }

    public MailAccountCredential(Guid id, Guid mailAccountId, AuthType authType)
    {
        Id = id;
        MailAccountId = mailAccountId;
        AuthType = authType;
        CreationTime = DateTime.UtcNow;
    }
}
