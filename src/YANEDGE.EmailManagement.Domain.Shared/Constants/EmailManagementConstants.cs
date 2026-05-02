namespace YANEDGE.EmailManagement.Constants;

/// <summary>
/// 系统常量
/// </summary>
public static class EmailManagementConstants
{
    /// <summary>
    /// 最大邮件主题长度
    /// </summary>
    public const int MaxSubjectLength = 500;

    /// <summary>
    /// 最大邮箱地址长度
    /// </summary>
    public const int MaxEmailAddressLength = 256;

    /// <summary>
    /// 最大显示名称长度
    /// </summary>
    public const int MaxDisplayNameLength = 200;

    /// <summary>
    /// 最大附件文件名长度
    /// </summary>
    public const int MaxFileNameLength = 255;

    /// <summary>
    /// 默认最大重试次数
    /// </summary>
    public const int DefaultMaxRetryCount = 3;

    /// <summary>
    /// 默认连接超时(秒)
    /// </summary>
    public const int DefaultConnectionTimeoutSeconds = 30;

    /// <summary>
    /// 默认操作超时(秒)
    /// </summary>
    public const int DefaultOperationTimeoutSeconds = 60;
}
