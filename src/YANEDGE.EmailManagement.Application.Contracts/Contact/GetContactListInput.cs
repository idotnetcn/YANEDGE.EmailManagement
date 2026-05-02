using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Contact;

public class GetContactListInput : PagedAndSortedResultRequestDto
{
    public string? Keyword { get; set; }

    public string? Source { get; set; }

    public bool? IsVerified { get; set; }

    public bool? IsActive { get; set; }

    public string? CustomerId { get; set; }

    public string? SupplierId { get; set; }
}
