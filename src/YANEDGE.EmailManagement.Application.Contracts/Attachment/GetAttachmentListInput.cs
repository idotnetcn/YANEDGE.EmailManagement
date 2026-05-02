using System;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Attachment;

public class GetAttachmentListInput : PagedAndSortedResultRequestDto
{
    public Guid? MailMessageId { get; set; }

    public string? Keyword { get; set; }

    public bool? IsSensitive { get; set; }

    public bool? IsScanned { get; set; }

    public bool? IsSafe { get; set; }
}
