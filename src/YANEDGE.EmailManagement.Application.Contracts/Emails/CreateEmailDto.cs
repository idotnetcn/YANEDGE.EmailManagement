using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.Emails;

public class CreateEmailDto
{
    [Required]
    [MaxLength(500)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    [EmailAddress]
    public string FromAddress { get; set; } = string.Empty;

    [MaxLength(256)]
    public string FromDisplayName { get; set; } = string.Empty;

    public bool IsBodyHtml { get; set; } = true;
    public EmailPriority Priority { get; set; } = EmailPriority.Normal;

    public List<CreateEmailRecipientDto> Recipients { get; set; } = new();
}
