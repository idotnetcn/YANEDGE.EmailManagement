using System;
using Volo.Abp.Application.Dtos;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Template;

public class MailTemplateDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Category { get; set; }

    public string? Language { get; set; }

    public string? Description { get; set; }

    public string SubjectTemplate { get; set; } = null!;

    public string BodyTemplate { get; set; } = null!;

    public string? PlainTextTemplate { get; set; }

    public TemplateStatus Status { get; set; }

    public int Version { get; set; }

    public bool RequiresApproval { get; set; }

    public bool IsDefault { get; set; }

    public int SortOrder { get; set; }
}
