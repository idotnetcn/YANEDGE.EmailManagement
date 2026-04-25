using System;
using System.ComponentModel.DataAnnotations;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.MailTemplates;

public class CreateMailTemplateInput
{
    [Required, MaxLength(64)]
    public string Code { get; set; } = null!;

    [Required, MaxLength(128)]
    public string Name { get; set; } = null!;

    public Guid? CategoryId { get; set; }

    [MaxLength(512)]
    public string? SubjectTemplate { get; set; }

    public BodyFormat BodyFormat { get; set; } = BodyFormat.Html;

    [MaxLength(16)]
    public string? LanguageCode { get; set; }

    public string? BodyContent { get; set; }
    public bool NeedApproval { get; set; }

    [MaxLength(512)]
    public string? Description { get; set; }
}
