namespace YANEDGE.EmailManagement.Enums;

/// <summary>
/// 规则动作类型
/// </summary>
public enum RuleActionType
{
    /// <summary>
    /// 打标签
    /// </summary>
    AddLabel = 0,

    /// <summary>
    /// 自动分派
    /// </summary>
    AutoAssign = 10,

    /// <summary>
    /// 标记重要
    /// </summary>
    MarkImportant = 20,

    /// <summary>
    /// 自动归档
    /// </summary>
    AutoArchive = 30,

    /// <summary>
    /// 进入垃圾箱
    /// </summary>
    MoveToJunk = 40,

    /// <summary>
    /// 设置优先级
    /// </summary>
    SetPriority = 50
}
