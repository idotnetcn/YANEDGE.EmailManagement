using System;
using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Conversations;

public class MailRecipient : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailMessageId { get; private set; }
    public RecipientType RecipientType { get; set; }
    public string EmailAddress { get; set; } = null!;
    public string? DisplayName { get; set; }
    public int RecipientOrder { get; set; } = 1;
    public DateTime CreationTime { get; set; }

    protected MailRecipient() { }

    public MailRecipient(Guid id, Guid mailMessageId, RecipientType type, string emailAddress)
    {
        Id = id;
        MailMessageId = mailMessageId;
        RecipientType = type;
        EmailAddress = emailAddress;
        CreationTime = DateTime.UtcNow;
    }
}
