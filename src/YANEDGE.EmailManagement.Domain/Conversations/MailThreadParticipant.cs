using System;
using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Conversations;

public class MailThreadParticipant : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid ThreadId { get; private set; }
    public Guid UserId { get; private set; }
    public ParticipantType ParticipantType { get; set; }
    public PermissionSourceType SourceType { get; set; } = PermissionSourceType.Manual;
    public DateTime JoinedTime { get; set; }
    public DateTime? LeftTime { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Remark { get; set; }

    protected MailThreadParticipant() { }

    public MailThreadParticipant(Guid id, Guid threadId, Guid userId, ParticipantType participantType)
    {
        Id = id;
        ThreadId = threadId;
        UserId = userId;
        ParticipantType = participantType;
        JoinedTime = DateTime.UtcNow;
    }
}
