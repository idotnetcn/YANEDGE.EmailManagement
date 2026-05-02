using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.MailMessage;

/// <summary>
/// 邮件消息聚合根
/// </summary>
public class MailMessage : AggregateRoot<Guid>
{
    /// <summary>
    /// 邮箱账号ID
    /// </summary>
    public Guid MailAccountId { get; private set; }

    /// <summary>
    /// 线程ID
    /// </summary>
    public Guid? ThreadId { get; private set; }

    /// <summary>
    /// Internet Message ID
    /// </summary>
    public string? InternetMessageId { get; private set; }

    /// <summary>
    /// 邮件主题
    /// </summary>
    public string Subject { get; private set; }

    /// <summary>
    /// 邮件方向
    /// </summary>
    public MailDirection Direction { get; private set; }

    /// <summary>
    /// 发件人地址
    /// </summary>
    public string FromAddress { get; private set; }

    /// <summary>
    /// 发件人显示名
    /// </summary>
    public string? FromDisplayName { get; private set; }

    /// <summary>
    /// 收件时间(对于收件)
    /// </summary>
    public DateTime? ReceivedTime { get; private set; }

    /// <summary>
    /// 发件时间(对于发件)
    /// </summary>
    public DateTime? SentTime { get; private set; }

    /// <summary>
    /// 是否有附件
    /// </summary>
    public bool HasAttachment { get; private set; }

    /// <summary>
    /// 是否已读
    /// </summary>
    public bool IsRead { get; private set; }

    /// <summary>
    /// 重要性
    /// </summary>
    public int Importance { get; private set; }

    /// <summary>
    /// HTML正文(已净化)
    /// </summary>
    public string? SanitizedHtmlBody { get; private set; }

    /// <summary>
    /// 纯文本正文
    /// </summary>
    public string? TextBody { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    private MailMessage()
    {
        // For ORM
        Subject = string.Empty;
        FromAddress = string.Empty;
    }

    public MailMessage(
        Guid id,
        Guid mailAccountId,
        string subject,
        MailDirection direction,
        string fromAddress,
        string? fromDisplayName = null) : base(id)
    {
        MailAccountId = mailAccountId;
        Subject = subject;
        Direction = direction;
        FromAddress = fromAddress;
        FromDisplayName = fromDisplayName;
        IsRead = false;
        HasAttachment = false;
        Importance = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetThread(Guid threadId)
    {
        ThreadId = threadId;
    }

    public void SetInternetMessageId(string messageId)
    {
        InternetMessageId = messageId;
    }

    public void SetReceivedTime(DateTime receivedTime)
    {
        ReceivedTime = receivedTime;
    }

    public void SetSentTime(DateTime sentTime)
    {
        SentTime = sentTime;
    }

    public void SetBody(string? htmlBody, string? textBody)
    {
        SanitizedHtmlBody = htmlBody;
        TextBody = textBody;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }

    public void MarkAsUnread()
    {
        IsRead = false;
    }

    public void SetHasAttachment(bool hasAttachment)
    {
        HasAttachment = hasAttachment;
    }

    public void SetImportance(int importance)
    {
        Importance = importance;
    }
}
