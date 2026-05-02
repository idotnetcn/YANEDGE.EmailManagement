namespace YANEDGE.EmailManagement.Enums;

/// <summary>
/// 邮件模板状态
/// </summary>
public enum TemplateStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 0,

    /// <summary>
    /// 待审批
    /// </summary>
    PendingApproval = 10,

    /// <summary>
    /// 已启用
    /// </summary>
    Active = 20,

    /// <summary>
    /// 已停用
    /// </summary>
    Inactive = 30,

    /// <summary>
    /// 已归档
    /// </summary>
    Archived = 40
}
