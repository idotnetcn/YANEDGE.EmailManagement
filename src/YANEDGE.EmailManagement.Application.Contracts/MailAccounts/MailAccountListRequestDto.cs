using System;
using YANEDGE.EmailManagement.Enums;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.MailAccounts;

public class MailAccountListRequestDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public AccountType? AccountType { get; set; }
    public AccountStatus? Status { get; set; }
    public Guid? OwnerUserId { get; set; }
}
