using System;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Label;

public class MailLabelDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;

    public string Color { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsSystemLabel { get; set; }

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }

    public Guid? OwnerUserId { get; set; }

    public Guid? OwnerOrganizationId { get; set; }
}
