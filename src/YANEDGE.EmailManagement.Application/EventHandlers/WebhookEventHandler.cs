using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using YANEDGE.EmailManagement.Domain.Shared.Events;
using YANEDGE.EmailManagement.Domain.Webhook;
using YANEDGE.EmailManagement.Services.Implementation;

namespace YANEDGE.EmailManagement.Application.EventHandlers;

/// <summary>
/// Webhook事件处理器
/// Handles all domain events and triggers webhook deliveries
/// </summary>
public class WebhookEventHandler :
    IDistributedEventHandler<MailMessageReceivedEvent>,
    IDistributedEventHandler<MailMessageSentEvent>,
    IDistributedEventHandler<MailSendTaskCreatedEvent>,
    IDistributedEventHandler<MailSendTaskFailedEvent>,
    IDistributedEventHandler<MailThreadStatusChangedEvent>,
    IDistributedEventHandler<MailApprovalRequestedEvent>,
    IDistributedEventHandler<MailApprovalCompletedEvent>,
    IDistributedEventHandler<MailAttachmentAccessedEvent>,
    IDistributedEventHandler<MailRuleExecutedEvent>,
    ITransientDependency
{
    private readonly IWebhookSubscriptionRepository _subscriptionRepository;
    private readonly WebhookDeliveryService _deliveryService;
    private readonly ILogger<WebhookEventHandler> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public WebhookEventHandler(
        IWebhookSubscriptionRepository subscriptionRepository,
        WebhookDeliveryService deliveryService,
        ILogger<WebhookEventHandler> logger,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _subscriptionRepository = subscriptionRepository;
        _deliveryService = deliveryService;
        _logger = logger;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task HandleEventAsync(MailMessageReceivedEvent eventData)
    {
        await ProcessWebhookAsync("EmailManagement.MailMessage.Received", eventData.MessageId, eventData);
    }

    public async Task HandleEventAsync(MailMessageSentEvent eventData)
    {
        await ProcessWebhookAsync("EmailManagement.MailMessage.Sent", eventData.MessageId ?? eventData.SendTaskId, eventData);
    }

    public async Task HandleEventAsync(MailSendTaskCreatedEvent eventData)
    {
        await ProcessWebhookAsync("EmailManagement.MailSendTask.Created", eventData.SendTaskId, eventData);
    }

    public async Task HandleEventAsync(MailSendTaskFailedEvent eventData)
    {
        await ProcessWebhookAsync("EmailManagement.MailSendTask.Failed", eventData.SendTaskId, eventData);
    }

    public async Task HandleEventAsync(MailThreadStatusChangedEvent eventData)
    {
        await ProcessWebhookAsync("EmailManagement.MailThread.StatusChanged", eventData.ThreadId, eventData);
    }

    public async Task HandleEventAsync(MailApprovalRequestedEvent eventData)
    {
        await ProcessWebhookAsync("EmailManagement.MailApproval.Requested", eventData.ApprovalId, eventData);
    }

    public async Task HandleEventAsync(MailApprovalCompletedEvent eventData)
    {
        await ProcessWebhookAsync("EmailManagement.MailApproval.Completed", eventData.ApprovalId, eventData);
    }

    public async Task HandleEventAsync(MailAttachmentAccessedEvent eventData)
    {
        await ProcessWebhookAsync("EmailManagement.MailAttachment.Accessed", eventData.AttachmentId, eventData);
    }

    public async Task HandleEventAsync(MailRuleExecutedEvent eventData)
    {
        await ProcessWebhookAsync("EmailManagement.MailRule.Executed", eventData.RuleId, eventData);
    }

    /// <summary>
    /// 处理Webhook投递
    /// </summary>
    private async Task ProcessWebhookAsync(string eventType, Guid eventId, object eventData)
    {
        try
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true);

            // 获取订阅该事件类型的所有活跃订阅
            var subscriptions = await _subscriptionRepository.GetActiveSubscriptionsByEventTypeAsync(eventType);

            if (subscriptions.Count == 0)
            {
                _logger.LogDebug(
                    "No active subscriptions found for event type: {EventType}",
                    eventType);
                await uow.CompleteAsync();
                return;
            }

            _logger.LogInformation(
                "Processing webhook for event type: {EventType}, EventId: {EventId}, Subscriptions: {Count}",
                eventType, eventId, subscriptions.Count);

            // 为每个订阅创建投递日志
            foreach (var subscription in subscriptions)
            {
                try
                {
                    var deliveryLog = await _deliveryService.CreateDeliveryLogAsync(
                        subscription,
                        eventId,
                        eventType,
                        eventData);

                    _logger.LogDebug(
                        "Created webhook delivery log: {DeliveryId} for subscription: {SubscriptionId}",
                        deliveryLog.Id, subscription.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to create webhook delivery log for subscription: {SubscriptionId}, EventType: {EventType}",
                        subscription.Id, eventType);
                    // 继续处理下一个订阅
                }
            }

            await uow.CompleteAsync();

            _logger.LogInformation(
                "Webhook delivery logs created for event type: {EventType}, EventId: {EventId}",
                eventType, eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to process webhook for event type: {EventType}, EventId: {EventId}",
                eventType, eventId);
            // 不抛出异常，避免影响主流程
        }
    }
}
