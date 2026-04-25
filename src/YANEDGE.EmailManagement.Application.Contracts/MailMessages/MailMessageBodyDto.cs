using System;

namespace YANEDGE.EmailManagement.MailMessages;

public class MailMessageBodyDto
{
    public Guid MailMessageId { get; set; }
    public string? BodyText { get; set; }
    public string? BodyHtmlRaw { get; set; }
    public string? BodyHtmlSafe { get; set; }
}
