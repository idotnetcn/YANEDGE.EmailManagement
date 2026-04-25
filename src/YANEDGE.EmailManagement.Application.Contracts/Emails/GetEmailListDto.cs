using System;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Emails;

public class GetEmailListDto : PagedAndSortedResultRequestDto
{
    public EmailStatus? Status { get; set; }
    public EmailPriority? Priority { get; set; }
    public string? FromAddress { get; set; }
    public DateTime? ScheduledBefore { get; set; }
}
