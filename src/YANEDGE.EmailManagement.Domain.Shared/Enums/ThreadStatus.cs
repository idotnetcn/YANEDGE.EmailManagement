namespace YANEDGE.EmailManagement.Enums;

/// <summary>
/// 线程状态
/// </summary>
public enum ThreadStatus
{
    /// <summary>
    /// 待分派
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 待处理
    /// </summary>
    Todo = 2,

    /// <summary>
    /// 处理中
    /// </summary>
    Processing = 3,

    /// <summary>
    /// 待审批
    /// </summary>
    WaitingForApproval = 4,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 5,

    /// <summary>
    /// 已归档
    /// </summary>
    Archived = 6,

    /// <summary>
    /// 已关闭
    /// </summary>
    Closed = 7
}
