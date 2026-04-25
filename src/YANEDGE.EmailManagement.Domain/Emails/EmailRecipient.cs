using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Emails;

public class EmailRecipient : Entity<Guid>
{
    public Guid EmailId { get; private set; }
    public string Address { get; private set; } = null!;
    public string DisplayName { get; private set; } = string.Empty;
    public RecipientType RecipientType { get; private set; }

    protected EmailRecipient() { }

    public EmailRecipient(Guid id, Guid emailId, string address, RecipientType recipientType, string displayName = "")
        : base(id)
    {
        EmailId = emailId;
        Address = Check.NotNullOrWhiteSpace(address, nameof(address), EmailConsts.MaxAddressLength);
        DisplayName = displayName;
        RecipientType = recipientType;
    }
}
