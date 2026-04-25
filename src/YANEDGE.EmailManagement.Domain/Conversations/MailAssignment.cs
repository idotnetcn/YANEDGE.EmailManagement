using System;
using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Conversations;

public class MailAssignment : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid ThreadId { get; private set; }
    public Guid? MailMessageId { get; set; }
    public Guid? FromUserId { get; set; }
    public Guid? ToUserId { get; set; }
    public Guid? ToRoleId { get; set; }
    public Guid? ToOrganizationUnitId { get; set; }
    public AssignmentType AssignmentType { get; set; }
    public AssignmentStatus Status { get; set; }
    public DateTime AssignedTime { get; set; }
    public DateTime? AcceptedTime { get; set; }
    public DateTime? FinishedTime { get; set; }
    public string? Comment { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }

    protected MailAssignment() { }

    public MailAssignment(Guid id, Guid threadId, AssignmentType assignmentType)
    {
        Id = id;
        ThreadId = threadId;
        AssignmentType = assignmentType;
        Status = AssignmentStatus.Pending;
        AssignedTime = DateTime.UtcNow;
        CreationTime = DateTime.UtcNow;
    }
}
