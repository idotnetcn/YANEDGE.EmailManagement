using System;
using YANEDGE.EmailManagement.Enums;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.MailThreads;

public class MailThreadListRequestDto : PagedAndSortedResultRequestDto
{
    public Guid? MailAccountId { get; set; }
    public ThreadStatus? Status { get; set; }
    public Guid? OwnerUserId { get; set; }
    public string? Filter { get; set; }
}
