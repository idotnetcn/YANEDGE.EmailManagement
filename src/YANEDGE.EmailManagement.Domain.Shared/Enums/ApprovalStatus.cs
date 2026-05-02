namespace YANEDGE.EmailManagement.Enums;

/// <summary>
/// 审批状态
/// </summary>
public enum ApprovalStatus
{
    /// <summary>
    /// 待审批
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 已通过
    /// </summary>
    Approved = 2,

    /// <summary>
    /// 已拒绝
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// 已撤回
    /// </summary>
    Withdrawn = 4
}
