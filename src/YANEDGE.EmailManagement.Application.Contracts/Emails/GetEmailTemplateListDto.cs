using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Emails;

public class GetEmailTemplateListDto : PagedAndSortedResultRequestDto
{
    public bool? IsActive { get; set; }
    public string? Filter { get; set; }
}
