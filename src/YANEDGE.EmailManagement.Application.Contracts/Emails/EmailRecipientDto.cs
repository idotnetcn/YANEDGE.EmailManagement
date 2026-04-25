using System;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Emails;

public class EmailRecipientDto : EntityDto<Guid>
{
    public string Address { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public RecipientType RecipientType { get; set; }
}
