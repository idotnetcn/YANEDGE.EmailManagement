using System;
using YANEDGE.EmailManagement.Enums;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.MailTemplates;

public class MailTemplateDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Guid? CategoryId { get; set; }
    public string? SubjectTemplate { get; set; }
    public BodyFormat BodyFormat { get; set; }
    public string? LanguageCode { get; set; }
    public int CurrentVersionNo { get; set; }
    public bool IsEnabled { get; set; }
    public bool NeedApproval { get; set; }
    public string? Description { get; set; }
}
