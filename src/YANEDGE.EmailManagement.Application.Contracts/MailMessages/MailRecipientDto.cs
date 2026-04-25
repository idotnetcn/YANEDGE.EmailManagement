using System;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.MailMessages;

public class MailRecipientDto
{
    public Guid Id { get; set; }
    public RecipientType RecipientType { get; set; }
    public string EmailAddress { get; set; } = null!;
    public string? DisplayName { get; set; }
}
