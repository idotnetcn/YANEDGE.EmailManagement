using System;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Label;

public class GetLabelListInput : PagedAndSortedResultRequestDto
{
    public string? Keyword { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSystemLabel { get; set; }

    public Guid? OwnerUserId { get; set; }

    public Guid? OwnerOrganizationId { get; set; }
}
