using YANEDGE.EmailManagement.Enums;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Contacts;

public class ContactListRequestDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public ContactType? ContactType { get; set; }
    public bool? IsEnabled { get; set; }
    public string? SourceSystem { get; set; }
}
