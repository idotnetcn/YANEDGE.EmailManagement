namespace YANEDGE.EmailManagement.Enums;

/// <summary>
/// 规则条件类型
/// </summary>
public enum RuleConditionType
{
    /// <summary>
    /// 发件人地址
    /// </summary>
    SenderAddress = 0,

    /// <summary>
    /// 发件人域名
    /// </summary>
    SenderDomain = 10,

    /// <summary>
    /// 收件邮箱
    /// </summary>
    RecipientMailbox = 20,

    /// <summary>
    /// 主题包含关键词
    /// </summary>
    SubjectContains = 30,

    /// <summary>
    /// 正文包含关键词
    /// </summary>
    BodyContains = 40,

    /// <summary>
    /// 是否有附件
    /// </summary>
    HasAttachment = 50,

    /// <summary>
    /// 附件类型
    /// </summary>
    AttachmentType = 60,

    /// <summary>
    /// 时间范围
    /// </summary>
    TimeRange = 70
}
