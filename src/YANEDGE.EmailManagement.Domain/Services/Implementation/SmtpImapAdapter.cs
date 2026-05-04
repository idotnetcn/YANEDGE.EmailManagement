using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using MailAccountEntity = YANEDGE.EmailManagement.Domain.MailAccount.MailAccount;
using MailMessageEntity = YANEDGE.EmailManagement.Domain.MailMessage.MailMessage;
using YANEDGE.EmailManagement.Domain.Services;
using MailKit.Net.Smtp;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Services.Implementation;

/// <summary>
/// SMTP/IMAP协议适配器实现
/// 使用 MailKit 实现真实的邮件协议通信
/// </summary>
public class SmtpImapAdapter : IMailProtocolAdapter, ITransientDependency
{
    private readonly ILogger<SmtpImapAdapter> _logger;
    private readonly IPasswordEncryptionService _encryptionService;

    public SmtpImapAdapter(
        ILogger<SmtpImapAdapter> logger,
        IPasswordEncryptionService encryptionService)
    {
        _logger = logger;
        _encryptionService = encryptionService;
    }

    public async Task<bool> TestConnectionAsync(MailAccountEntity account, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Testing IMAP/SMTP connection for account: {EmailAddress}", account.EmailAddress);

            // 解密密码
            var password = _encryptionService.Decrypt(account.EncryptedPassword);

            var imapSuccess = false;
            var smtpSuccess = false;

            // 测试IMAP连接
            if (account.IncomingProtocol == MailProtocol.IMAP)
            {
                imapSuccess = await TestImapConnectionAsync(account, password, cancellationToken);
            }
            else
            {
                _logger.LogWarning("Only IMAP protocol is currently supported for incoming mail. Protocol: {Protocol}", account.IncomingProtocol);
                imapSuccess = true; // Skip IMAP test if not IMAP protocol
            }

            // 测试SMTP连接
            smtpSuccess = await TestSmtpConnectionAsync(account, password, cancellationToken);

            var success = imapSuccess && smtpSuccess;
            _logger.LogInformation("Connection test {Result} for account: {EmailAddress}",
                success ? "successful" : "failed", account.EmailAddress);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connection test failed for account: {EmailAddress}", account.EmailAddress);
            return false;
        }
    }

    private async Task<bool> TestImapConnectionAsync(MailAccountEntity account, string password, CancellationToken cancellationToken)
    {
        using var client = new ImapClient();
        try
        {
            _logger.LogDebug("Connecting to IMAP server {Host}:{Port}", account.IncomingHost, account.IncomingPort);

            var secureSocketOptions = account.IncomingSslEnabled
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;

            await client.ConnectAsync(account.IncomingHost, account.IncomingPort, secureSocketOptions, cancellationToken);
            await client.AuthenticateAsync(account.Username, password, cancellationToken);

            _logger.LogDebug("IMAP connection successful for {EmailAddress}", account.EmailAddress);

            await client.DisconnectAsync(true, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "IMAP connection failed for {EmailAddress}", account.EmailAddress);
            return false;
        }
    }

    private async Task<bool> TestSmtpConnectionAsync(MailAccountEntity account, string password, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();
        try
        {
            _logger.LogDebug("Connecting to SMTP server {Host}:{Port}", account.OutgoingHost, account.OutgoingPort);

            var secureSocketOptions = account.OutgoingSslEnabled
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;

            await client.ConnectAsync(account.OutgoingHost, account.OutgoingPort, secureSocketOptions, cancellationToken);
            await client.AuthenticateAsync(account.Username, password, cancellationToken);

            _logger.LogDebug("SMTP connection successful for {EmailAddress}", account.EmailAddress);

            await client.DisconnectAsync(true, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP connection failed for {EmailAddress}", account.EmailAddress);
            return false;
        }
    }

    public async Task<List<MailMessageEntity>> SyncMailsAsync(
        MailAccountEntity account,
        DateTime? since = null,
        CancellationToken cancellationToken = default)
    {
        using var client = new ImapClient();
        try
        {
            _logger.LogInformation("Syncing emails for account: {EmailAddress}", account.EmailAddress);

            // 解密密码
            var password = _encryptionService.Decrypt(account.EncryptedPassword);

            // 连接到IMAP服务器
            var secureSocketOptions = account.IncomingSslEnabled
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;

            await client.ConnectAsync(account.IncomingHost, account.IncomingPort, secureSocketOptions, cancellationToken);
            await client.AuthenticateAsync(account.Username, password, cancellationToken);

            // 打开收件箱
            var inbox = client.Inbox;
            if (inbox == null)
            {
                throw new InvalidOperationException("IMAP Inbox is not available");
            }
            await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly, cancellationToken);

            // 构建搜索条件
            SearchQuery query;
            if (since.HasValue)
            {
                query = SearchQuery.DeliveredAfter(since.Value);
            }
            else
            {
                // 如果没有指定时间，获取最近30天的邮件
                query = SearchQuery.DeliveredAfter(DateTime.UtcNow.AddDays(-30));
            }

            var uids = await inbox.SearchAsync(query, cancellationToken);

            var messages = new List<MailMessageEntity>();

            // 限制每次同步的邮件数量
            var limit = Math.Min(uids.Count, 100);
            _logger.LogDebug("Found {Count} emails to sync, processing {Limit}", uids.Count, limit);

            for (int i = 0; i < limit; i++)
            {
                try
                {
                    var mimeMessage = await inbox.GetMessageAsync(uids[i], cancellationToken);

                    var mailMessage = ConvertToMailMessage(account.Id, mimeMessage);
                    messages.Add(mailMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to process message UID {Uid}", uids[i]);
                    // Continue processing other messages
                }
            }

            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Synced {Count} emails for account: {EmailAddress}", messages.Count, account.EmailAddress);
            return messages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email sync failed for account: {EmailAddress}", account.EmailAddress);
            throw;
        }
    }

    private MailMessageEntity ConvertToMailMessage(Guid accountId, MimeMessage mimeMessage)
    {
        var fromAddress = mimeMessage.From.Mailboxes.FirstOrDefault();
        var message = new MailMessageEntity(
            id: Guid.NewGuid(),
            mailAccountId: accountId,
            subject: mimeMessage.Subject ?? "(No Subject)",
            direction: MailDirection.Inbound,
            fromAddress: fromAddress?.Address ?? "",
            fromDisplayName: fromAddress?.Name
        );

        // 设置消息ID
        if (!string.IsNullOrEmpty(mimeMessage.MessageId))
        {
            message.SetInternetMessageId(mimeMessage.MessageId);
        }

        // 设置接收时间
        if (mimeMessage.Date != DateTimeOffset.MinValue)
        {
            message.SetReceivedTime(mimeMessage.Date.UtcDateTime);
        }

        // 设置正文
        var htmlBody = mimeMessage.HtmlBody;
        var textBody = mimeMessage.TextBody;
        message.SetBody(htmlBody, textBody);

        // 检查是否有附件
        var hasAttachment = mimeMessage.Attachments.Any();
        message.SetHasAttachment(hasAttachment);

        // 设置重要性
        var importance = mimeMessage.Importance switch
        {
            MessageImportance.Low => -1,
            MessageImportance.Normal => 0,
            MessageImportance.High => 1,
            _ => 0
        };
        message.SetImportance(importance);

        return message;
    }

    public async Task<bool> SendMailAsync(
        MailAccountEntity account,
        MailSendRequest request,
        CancellationToken cancellationToken = default)
    {
        using var client = new SmtpClient();
        try
        {
            _logger.LogInformation("Sending email from account: {EmailAddress}", account.EmailAddress);

            // 解密密码
            var password = _encryptionService.Decrypt(account.EncryptedPassword);

            // 创建邮件消息
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(account.DisplayName, account.EmailAddress));

            // 添加收件人
            foreach (var to in request.ToAddresses)
            {
                message.To.Add(MailboxAddress.Parse(to));
            }

            // 添加抄送
            if (request.CcAddresses != null)
            {
                foreach (var cc in request.CcAddresses)
                {
                    message.Cc.Add(MailboxAddress.Parse(cc));
                }
            }

            // 添加密送
            if (request.BccAddresses != null)
            {
                foreach (var bcc in request.BccAddresses)
                {
                    message.Bcc.Add(MailboxAddress.Parse(bcc));
                }
            }

            message.Subject = request.Subject;

            // 构建邮件正文
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = request.HtmlBody;

            if (!string.IsNullOrEmpty(request.PlainTextBody))
            {
                bodyBuilder.TextBody = request.PlainTextBody;
            }

            // 添加附件
            if (request.Attachments != null)
            {
                foreach (var attachment in request.Attachments)
                {
                    if (attachment.IsInline && !string.IsNullOrEmpty(attachment.ContentId))
                    {
                        var image = bodyBuilder.LinkedResources.Add(attachment.FileName, attachment.Content);
                        image.ContentId = attachment.ContentId;
                    }
                    else
                    {
                        bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
                    }
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            // 连接到SMTP服务器并发送
            var secureSocketOptions = account.OutgoingSslEnabled
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;

            await client.ConnectAsync(account.OutgoingHost, account.OutgoingPort, secureSocketOptions, cancellationToken);
            await client.AuthenticateAsync(account.Username, password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent successfully from account: {EmailAddress}", account.EmailAddress);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email sending failed for account: {EmailAddress}", account.EmailAddress);
            throw;
        }
    }
}
