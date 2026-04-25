using System;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Emails;

public class EmailTemplateDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsBodyHtml { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}
