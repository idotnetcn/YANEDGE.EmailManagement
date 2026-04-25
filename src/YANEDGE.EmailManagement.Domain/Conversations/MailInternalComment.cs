using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Conversations;

public class MailInternalComment : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid ThreadId { get; private set; }
    public Guid? MailMessageId { get; set; }
    public Guid UserId { get; private set; }
    public string Content { get; set; } = null!;
    public bool IsPinned { get; set; }
    public string? MentionUserIds { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }

    protected MailInternalComment() { }

    public MailInternalComment(Guid id, Guid threadId, Guid userId, string content)
    {
        Id = id;
        ThreadId = threadId;
        UserId = userId;
        Content = content;
        CreationTime = DateTime.UtcNow;
    }
}
