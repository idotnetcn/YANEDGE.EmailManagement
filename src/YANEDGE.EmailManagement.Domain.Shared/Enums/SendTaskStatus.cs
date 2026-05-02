namespace YANEDGE.EmailManagement.Enums;

/// <summary>
/// 发件任务状态
/// </summary>
public enum SendTaskStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 1,

    /// <summary>
    /// 待审批
    /// </summary>
    PendingApproval = 2,

    /// <summary>
    /// 待发送
    /// </summary>
    PendingSend = 3,

    /// <summary>
    /// 发送中
    /// </summary>
    Sending = 4,

    /// <summary>
    /// 发送成功
    /// </summary>
    Sent = 5,

    /// <summary>
    /// 发送失败
    /// </summary>
    Failed = 6,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 7
}
