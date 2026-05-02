using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.Collaboration;

/// <summary>
/// 线程分派记录
/// </summary>
public class ThreadAssignment : Entity<Guid>
{
    /// <summary>
    /// 线程ID
    /// </summary>
    public Guid ThreadId { get; private set; }

    /// <summary>
    /// 来源负责人类型
    /// </summary>
    public AssigneeType? FromAssigneeType { get; private set; }

    /// <summary>
    /// 来源负责人ID
    /// </summary>
    public Guid? FromAssigneeId { get; private set; }

    /// <summary>
    /// 目标负责人类型
    /// </summary>
    public AssigneeType ToAssigneeType { get; private set; }

    /// <summary>
    /// 目标负责人ID
    /// </summary>
    public Guid ToAssigneeId { get; private set; }

    /// <summary>
    /// 分派类型(认领/分派/转派)
    /// </summary>
    public string AssignmentType { get; private set; }

    /// <summary>
    /// 分派原因
    /// </summary>
    public string? Reason { get; private set; }

    /// <summary>
    /// 操作人ID
    /// </summary>
    public Guid OperatorId { get; private set; }

    /// <summary>
    /// 分派时间
    /// </summary>
    public DateTime AssignedAt { get; private set; }

    private ThreadAssignment()
    {
        // For ORM
        AssignmentType = string.Empty;
    }

    public ThreadAssignment(
        Guid id,
        Guid threadId,
        AssigneeType toAssigneeType,
        Guid toAssigneeId,
        string assignmentType,
        Guid operatorId,
        AssigneeType? fromAssigneeType = null,
        Guid? fromAssigneeId = null,
        string? reason = null) : base(id)
    {
        ThreadId = threadId;
        FromAssigneeType = fromAssigneeType;
        FromAssigneeId = fromAssigneeId;
        ToAssigneeType = toAssigneeType;
        ToAssigneeId = toAssigneeId;
        AssignmentType = assignmentType;
        OperatorId = operatorId;
        Reason = reason;
        AssignedAt = DateTime.UtcNow;
    }
}
