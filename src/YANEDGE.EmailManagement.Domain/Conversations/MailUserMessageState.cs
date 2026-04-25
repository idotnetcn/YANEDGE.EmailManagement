using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Conversations;

public class MailUserMessageState : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailMessageId { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsRead { get; set; }
    public bool IsStarred { get; set; }
    public bool IsArchived { get; set; }
    public bool IsHidden { get; set; }
    public bool IsPinned { get; set; }
    public byte? CustomImportance { get; set; }
    public DateTime? FirstReadTime { get; set; }
    public DateTime? ReadTime { get; set; }
    public DateTime? LastViewTime { get; set; }
    public DateTime? SnoozeUntil { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }

    protected MailUserMessageState() { }

    public MailUserMessageState(Guid id, Guid mailMessageId, Guid userId)
    {
        Id = id;
        MailMessageId = mailMessageId;
        UserId = userId;
        CreationTime = DateTime.UtcNow;
    }

    public void MarkRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadTime = DateTime.UtcNow;
            if (FirstReadTime == null) FirstReadTime = ReadTime;
        }
        LastViewTime = DateTime.UtcNow;
    }
}
