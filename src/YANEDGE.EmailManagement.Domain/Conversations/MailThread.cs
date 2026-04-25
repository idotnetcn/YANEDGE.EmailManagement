using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Conversations;

public class MailThread : FullAuditedAggregateRoot<Guid>
{
    public Guid? TenantId { get; private set; }
    public Guid MailAccountId { get; private set; }
    public string? Subject { get; private set; }
    public string? NormalizedSubject { get; private set; }
    public string ThreadKey { get; private set; } = null!;
    public DateTime? LatestMessageTime { get; private set; }
    public string? LatestSender { get; private set; }
    public int MessageCount { get; private set; }
    public int UnreadCount { get; private set; }
    public ThreadStatus Status { get; private set; }
    public Guid? OwnerUserId { get; private set; }
    public Guid? AssignedOrganizationUnitId { get; private set; }
    public Guid? AssignedRoleId { get; private set; }
    public DateTime? AssignmentTime { get; private set; }
    public Guid? LastAssignmentUserId { get; private set; }
    public Importance Priority { get; set; }
    public bool IsStarred { get; set; }
    public DateTime? FirstMessageTime { get; private set; }
    public DateTime? ClosedTime { get; private set; }
    public DateTime? ArchiveTime { get; private set; }
    public string? ExtraProperties { get; set; }

    public virtual ICollection<MailMessage> Messages { get; private set; } = new List<MailMessage>();
    public virtual ICollection<MailThreadParticipant> Participants { get; private set; } = new List<MailThreadParticipant>();
    public virtual ICollection<MailAssignment> Assignments { get; private set; } = new List<MailAssignment>();
    public virtual ICollection<MailInternalComment> Comments { get; private set; } = new List<MailInternalComment>();
    public virtual ICollection<MailProcessingRecord> ProcessingRecords { get; private set; } = new List<MailProcessingRecord>();
    public virtual ICollection<MailTodo> Todos { get; private set; } = new List<MailTodo>();

    protected MailThread() { }

    public MailThread(Guid id, Guid? tenantId, Guid mailAccountId, string threadKey, string? subject = null)
    {
        Id = id;
        TenantId = tenantId;
        MailAccountId = mailAccountId;
        ThreadKey = threadKey;
        Subject = subject;
        NormalizedSubject = subject?.ToUpperInvariant();
        Status = ThreadStatus.Active;
        Priority = Importance.Normal;
    }

    public void Assign(Guid? toUserId, Guid? toRoleId, Guid? toOuId, Guid? assignedBy)
    {
        OwnerUserId = toUserId;
        AssignedRoleId = toRoleId;
        AssignedOrganizationUnitId = toOuId;
        AssignmentTime = DateTime.UtcNow;
        LastAssignmentUserId = assignedBy;
    }

    public void Close()
    {
        Status = ThreadStatus.Closed;
        ClosedTime = DateTime.UtcNow;
    }

    public void Archive()
    {
        Status = ThreadStatus.Archived;
        ArchiveTime = DateTime.UtcNow;
    }

    public void Complete() => Status = ThreadStatus.Completed;

    public void IncrementMessageCount()
    {
        MessageCount++;
    }

    public void UpdateLatestMessage(DateTime time, string sender)
    {
        LatestMessageTime = time;
        LatestSender = sender;
        if (FirstMessageTime == null) FirstMessageTime = time;
    }
}
