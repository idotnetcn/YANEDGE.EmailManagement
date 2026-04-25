using System;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Emails;

public class EmailAttachmentDto : EntityDto<Guid>
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}
