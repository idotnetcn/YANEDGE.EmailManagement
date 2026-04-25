using System;
using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.MailTemplates;

public class UpdateMailTemplateInput
{
    [Required, MaxLength(128)]
    public string Name { get; set; } = null!;

    public Guid? CategoryId { get; set; }

    [MaxLength(512)]
    public string? SubjectTemplate { get; set; }

    public string? BodyContent { get; set; }
    public bool NeedApproval { get; set; }

    [MaxLength(512)]
    public string? Description { get; set; }
    public string? ChangeSummary { get; set; }
}
