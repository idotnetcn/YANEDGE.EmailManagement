using System;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.IntegrationApps;

public class IntegrationAppDto : FullAuditedEntityDto<Guid>
{
    public string AppId { get; set; } = null!;
    public string AppName { get; set; } = null!;
    public string? SourceSystem { get; set; }
    public string? CallbackUrl { get; set; }
    public bool IsEnabled { get; set; }
    public string? Description { get; set; }
}
