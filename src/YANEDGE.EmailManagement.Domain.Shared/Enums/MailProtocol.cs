namespace YANEDGE.EmailManagement.Enums;

/// <summary>
/// 邮件协议类型
/// </summary>
public enum MailProtocol
{
    /// <summary>
    /// IMAP
    /// </summary>
    IMAP = 1,

    /// <summary>
    /// POP3
    /// </summary>
    POP3 = 2,

    /// <summary>
    /// Exchange Web Services
    /// </summary>
    EWS = 3,

    /// <summary>
    /// Microsoft Graph API
    /// </summary>
    GraphApi = 4
}
