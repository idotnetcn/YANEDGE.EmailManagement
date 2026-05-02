using Volo.Abp.Application.Dtos;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Template;

public class GetTemplateListInput : PagedAndSortedResultRequestDto
{
    public string? Category { get; set; }

    public string? Keyword { get; set; }

    public TemplateStatus? Status { get; set; }
}
