using System;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.MailTemplates;

public class MailTemplateListRequestDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? IsEnabled { get; set; }
}
