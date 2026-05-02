using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Rule;

public class GetRuleListInput : PagedAndSortedResultRequestDto
{
    public string? Keyword { get; set; }

    public bool? IsActive { get; set; }
}
