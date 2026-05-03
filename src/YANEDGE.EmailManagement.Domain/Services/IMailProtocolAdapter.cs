using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MailAccountEntity = YANEDGE.EmailManagement.Domain.MailAccount.MailAccount;
using MailMessageEntity = YANEDGE.EmailManagement.Domain.MailMessage.MailMessage;

namespace YANEDGE.EmailManagement.Domain.Services;

/// <summary>
/// 邮件协议适配器接口
/// </summary>
public interface IMailProtocolAdapter
{
    /// <summary>
    /// 测试连接
    /// </summary>
    Task<bool> TestConnectionAsync(MailAccountEntity account, CancellationToken cancellationToken = default);

    /// <summary>
    /// 同步邮件
    /// </summary>
    Task<List<MailMessageEntity>> SyncMailsAsync(MailAccountEntity account, DateTime? since = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送邮件
    /// </summary>
    Task<bool> SendMailAsync(MailAccountEntity account, MailSendRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// 邮件发送请求
/// </summary>
public class MailSendRequest
{
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public string? PlainTextBody { get; set; }
    public List<string> ToAddresses { get; set; } = new();
    public List<string>? CcAddresses { get; set; }
    public List<string>? BccAddresses { get; set; }
    public List<AttachmentInfo>? Attachments { get; set; }
}

/// <summary>
/// 附件信息
/// </summary>
public class AttachmentInfo
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public bool IsInline { get; set; }
    public string? ContentId { get; set; }
}
