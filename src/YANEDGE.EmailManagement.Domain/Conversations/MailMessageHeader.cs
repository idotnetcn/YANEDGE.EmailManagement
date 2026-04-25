using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Conversations;

public class MailMessageHeader : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailMessageId { get; private set; }
    public string HeaderName { get; set; } = null!;
    public string? HeaderValue { get; set; }
    public DateTime CreationTime { get; set; }

    protected MailMessageHeader() { }

    public MailMessageHeader(Guid id, Guid mailMessageId, string headerName, string? headerValue)
    {
        Id = id;
        MailMessageId = mailMessageId;
        HeaderName = headerName;
        HeaderValue = headerValue;
        CreationTime = DateTime.UtcNow;
    }
}
