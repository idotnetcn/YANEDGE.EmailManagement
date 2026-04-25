using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Conversations;

public class MailMessageBody : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailMessageId { get; private set; }
    public string? BodyText { get; set; }
    public string? BodyHtmlRaw { get; set; }
    public string? BodyHtmlSafe { get; set; }
    public string? BodyHash { get; set; }
    public byte ParseStatus { get; set; }
    public string? ParseErrorMessage { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }

    protected MailMessageBody() { }

    public MailMessageBody(Guid id, Guid mailMessageId)
    {
        Id = id;
        MailMessageId = mailMessageId;
        CreationTime = DateTime.UtcNow;
    }
}
