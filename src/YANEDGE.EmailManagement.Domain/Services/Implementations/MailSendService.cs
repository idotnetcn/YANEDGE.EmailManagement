using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Domain.MailCompose;

namespace YANEDGE.EmailManagement.Domain.Services.Implementations;

/// <summary>
/// 邮件发送服务实现
/// </summary>
public class MailSendService : IMailSendService, ITransientDependency
{
    private readonly IRepository<MailSendTask, Guid> _sendTaskRepository;
    private readonly IRepository<MailAccount.MailAccount, Guid> _mailAccountRepository;
    private readonly IPasswordEncryptionService _passwordEncryptionService;

    public MailSendService(
        IRepository<MailSendTask, Guid> sendTaskRepository,
        IRepository<MailAccount.MailAccount, Guid> mailAccountRepository,
        IPasswordEncryptionService passwordEncryptionService)
    {
        _sendTaskRepository = sendTaskRepository;
        _mailAccountRepository = mailAccountRepository;
        _passwordEncryptionService = passwordEncryptionService;
    }

    public async Task QueueSendTaskAsync(MailSendTask sendTask)
    {
        // 验证邮箱账号
        var account = await _mailAccountRepository.GetAsync(sendTask.MailAccountId);
        if (!account.SendEnabled)
        {
            throw new InvalidOperationException("邮箱发送未启用");
        }

        // TODO: 集成Hangfire后，使用BackgroundJobManager排队后台任务
        // await _backgroundJobManager.EnqueueAsync(
        //     new SendMailJobArgs { SendTaskId = sendTask.Id });
    }

    public async Task ExecuteSendTaskAsync(Guid sendTaskId)
    {
        var sendTask = await _sendTaskRepository.GetAsync(sendTaskId);
        var account = await _mailAccountRepository.GetAsync(sendTask.MailAccountId);

        try
        {
            // 更新状态为发送中
            sendTask.StartSending();
            await _sendTaskRepository.UpdateAsync(sendTask);

            // 解密密码
            var plainPassword = _passwordEncryptionService.Decrypt(account.EncryptedPassword);

            // 构建邮件消息
            var message = BuildMimeMessage(sendTask, account);

            // 发送邮件
            using var client = new SmtpClient();

            var secureSocketOptions = account.OutgoingSslEnabled
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;

            await client.ConnectAsync(account.OutgoingHost, account.OutgoingPort, secureSocketOptions);

            if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
            {
                await client.AuthenticateAsync(account.Username, plainPassword);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            // 更新状态为已发送
            sendTask.MarkAsSent(DateTime.UtcNow);
            await _sendTaskRepository.UpdateAsync(sendTask);
        }
        catch (Exception ex)
        {
            // 更新状态为失败
            sendTask.MarkAsFailed("SEND_ERROR", $"发送失败: {ex.Message}");
            await _sendTaskRepository.UpdateAsync(sendTask);

            throw;
        }
    }

    private MimeMessage BuildMimeMessage(MailSendTask sendTask, MailAccount.MailAccount account)
    {
        var message = new MimeMessage();

        // 发件人
        message.From.Add(new MailboxAddress(account.DisplayName, account.EmailAddress));

        // TODO: 收件人/抄送/密送需要从MailSendRecipient表中加载
        // 这里先使用简单实现

        // 主题
        message.Subject = sendTask.Subject;

        // 邮件正文
        var builder = new BodyBuilder();

        if (!string.IsNullOrEmpty(sendTask.BodyHtml))
        {
            builder.HtmlBody = sendTask.BodyHtml;
            if (!string.IsNullOrEmpty(sendTask.BodyText))
            {
                builder.TextBody = sendTask.BodyText;
            }
        }
        else if (!string.IsNullOrEmpty(sendTask.BodyText))
        {
            builder.TextBody = sendTask.BodyText;
        }

        // TODO: 处理附件（需要从附件表中加载）

        message.Body = builder.ToMessageBody();

        return message;
    }
}
