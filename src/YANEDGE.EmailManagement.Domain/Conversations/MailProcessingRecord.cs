using System;
using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Conversations;

public class MailProcessingRecord : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid ThreadId { get; private set; }
    public Guid? MailMessageId { get; set; }
    public Guid? OperatorUserId { get; set; }
    public ProcessingStatus? OldStatus { get; set; }
    public ProcessingStatus NewStatus { get; set; }
    public string ActionName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreationTime { get; set; }

    protected MailProcessingRecord() { }

    public MailProcessingRecord(Guid id, Guid threadId, ProcessingStatus newStatus, string actionName)
    {
        Id = id;
        ThreadId = threadId;
        NewStatus = newStatus;
        ActionName = actionName;
        CreationTime = DateTime.UtcNow;
    }
}
