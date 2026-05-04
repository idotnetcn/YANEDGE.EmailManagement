using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using YANEDGE.EmailManagement.Domain.Services;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Domain.MailAccount;

namespace YANEDGE.EmailManagement.Services.Implementation;

/// <summary>
/// 邮件发送服务实现
/// </summary>
public class MailSendService : IMailSendService, ITransientDependency
{
    private readonly IMailProtocolAdapter _protocolAdapter;
    private readonly IMailSendTaskRepository _sendTaskRepository;
    private readonly IMailAccountRepository _mailAccountRepository;
    private readonly ILogger<MailSendService> _logger;

    public MailSendService(
        IMailProtocolAdapter protocolAdapter,
        IMailSendTaskRepository sendTaskRepository,
        IMailAccountRepository mailAccountRepository,
        ILogger<MailSendService> logger)
    {
        _protocolAdapter = protocolAdapter;
        _sendTaskRepository = sendTaskRepository;
        _mailAccountRepository = mailAccountRepository;
        _logger = logger;
    }

    public Task QueueSendTaskAsync(MailSendTask sendTask)
    {
        // 将发件任务加入队列
        // 实际实现需要使用后台任务队列（如 Hangfire）
        _logger.LogInformation("Queued send task {TaskId} for sending", sendTask.Id);
        return Task.CompletedTask;
    }

    public async Task ExecuteSendTaskAsync(Guid sendTaskId)
    {
        try
        {
            // 从数据库获取发件任务
            var sendTask = await _sendTaskRepository.GetAsync(sendTaskId);

            if (sendTask.Status != Enums.SendTaskStatus.PendingSend)
            {
                _logger.LogWarning("Send task {TaskId} is not in PendingSend status, current status: {Status}",
                    sendTaskId, sendTask.Status);
                return;
            }

            // 获取邮箱账号
            var account = await _mailAccountRepository.GetAsync(sendTask.MailAccountId);

            if (!account.SendEnabled)
            {
                _logger.LogWarning("Mail account {AccountId} has sending disabled", account.Id);
                sendTask.MarkAsFailed("ACCOUNT_DISABLED", "Mail account has sending disabled");
                await _sendTaskRepository.UpdateAsync(sendTask);
                return;
            }

            // 标记为发送中
            sendTask.StartSending();
            await _sendTaskRepository.UpdateAsync(sendTask);

            // 构建发送请求
            var sendRequest = new MailSendRequest
            {
                Subject = sendTask.Subject,
                HtmlBody = sendTask.BodyHtml ?? "",
                PlainTextBody = sendTask.BodyText,
                // Note: Recipients should be stored separately in MailSendTaskRecipient table
                // For now, we'll use a placeholder
                ToAddresses = new System.Collections.Generic.List<string>()
            };

            // TODO: Load recipients from MailSendTaskRecipient table
            // TODO: Load attachments from MailSendTaskAttachment table

            // 使用协议适配器发送邮件
            await _protocolAdapter.SendMailAsync(account, sendRequest, CancellationToken.None);

            // 更新发送状态
            sendTask.MarkAsSent(DateTime.UtcNow);
            account.UpdateLastSendTime(DateTime.UtcNow);

            await _sendTaskRepository.UpdateAsync(sendTask);
            await _mailAccountRepository.UpdateAsync(account);

            _logger.LogInformation("Send task {TaskId} completed successfully", sendTaskId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute send task {TaskId}", sendTaskId);

            try
            {
                var sendTask = await _sendTaskRepository.GetAsync(sendTaskId);
                sendTask.MarkAsFailed("SEND_ERROR", ex.Message);
                await _sendTaskRepository.UpdateAsync(sendTask);
            }
            catch (Exception updateEx)
            {
                _logger.LogError(updateEx, "Failed to update send task {TaskId} status after error", sendTaskId);
            }

            throw;
        }
    }
}
