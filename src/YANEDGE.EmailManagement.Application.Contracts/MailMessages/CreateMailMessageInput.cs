using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.MailMessages;

public class CreateMailMessageInput
{
    [Required]
    public Guid MailAccountId { get; set; }

    [MaxLength(512)]
    public string? Subject { get; set; }

    public List<RecipientInput> ToRecipients { get; set; } = new();
    public List<RecipientInput> CcRecipients { get; set; } = new();
    public List<RecipientInput> BccRecipients { get; set; } = new();

    public BodyFormat BodyFormat { get; set; } = BodyFormat.Html;
    public string? BodyHtml { get; set; }
    public string? BodyText { get; set; }

    public DateTime? ScheduledSendTime { get; set; }
    public SecurityLevel SecurityLevel { get; set; } = SecurityLevel.Normal;
    public Importance Importance { get; set; } = Importance.Normal;
    public bool NeedApproval { get; set; }
    public string? TemplateCode { get; set; }
    public Guid? TemplateId { get; set; }
}

public class RecipientInput
{
    [Required, MaxLength(256)]
    public string EmailAddress { get; set; } = null!;

    [MaxLength(256)]
    public string? DisplayName { get; set; }
}
