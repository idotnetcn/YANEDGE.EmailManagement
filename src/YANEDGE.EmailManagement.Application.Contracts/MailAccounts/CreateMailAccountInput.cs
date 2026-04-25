using System;
using System.ComponentModel.DataAnnotations;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.MailAccounts;

public class CreateMailAccountInput
{
    [Required, MaxLength(128)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(256)]
    public string EmailAddress { get; set; } = null!;

    [MaxLength(128)]
    public string? DisplayName { get; set; }

    public AccountType AccountType { get; set; } = AccountType.Personal;
    public OwnerType? OwnerType { get; set; }
    public Guid? OwnerUserId { get; set; }
    public VisibilityScope VisibilityScope { get; set; } = VisibilityScope.ExplicitOnly;
    public InboundProtocol InboundProtocol { get; set; } = InboundProtocol.IMAP;
    public OutboundProtocol OutboundProtocol { get; set; } = OutboundProtocol.SMTP;

    [MaxLength(256)]
    public string? ImapHost { get; set; }
    public int? ImapPort { get; set; }

    [MaxLength(256)]
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }

    public bool UseSsl { get; set; } = true;
    public int SyncIntervalSeconds { get; set; } = 300;

    [MaxLength(512)]
    public string? Description { get; set; }
}
