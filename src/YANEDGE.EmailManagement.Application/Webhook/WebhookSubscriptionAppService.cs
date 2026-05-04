using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Domain.Webhook;

namespace YANEDGE.EmailManagement.Webhook;

/// <summary>
/// Webhook订阅管理服务实现
/// </summary>
public class WebhookSubscriptionAppService : ApplicationService, IWebhookSubscriptionAppService
{
    private readonly IWebhookSubscriptionRepository _subscriptionRepository;

    public WebhookSubscriptionAppService(
        IWebhookSubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<WebhookSubscriptionDto> GetAsync(Guid id)
    {
        var subscription = await _subscriptionRepository.GetAsync(id);
        return ObjectMapper.Map<WebhookSubscription, WebhookSubscriptionDto>(subscription);
    }

    public async Task<PagedResultDto<WebhookSubscriptionDto>> GetListAsync(
        WebhookSubscriptionGetListInput input)
    {
        var queryable = await _subscriptionRepository.GetQueryableAsync();

        if (input.IsActive.HasValue)
        {
            queryable = queryable.Where(x => x.IsActive == input.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            queryable = queryable.Where(x =>
                x.Name.Contains(input.Filter) ||
                x.Url.Contains(input.Filter));
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = queryable
            .OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var subscriptions = await AsyncExecuter.ToListAsync(queryable);

        return new PagedResultDto<WebhookSubscriptionDto>(
            totalCount,
            ObjectMapper.Map<List<WebhookSubscription>, List<WebhookSubscriptionDto>>(subscriptions)
        );
    }

    public async Task<WebhookSubscriptionDto> CreateAsync(CreateWebhookSubscriptionDto input)
    {
        var subscription = new WebhookSubscription(
            GuidGenerator.Create(),
            input.Name,
            input.Url,
            input.Secret,
            input.SubscribedEvents,
            CurrentTenant.Id
        );

        subscription.Headers = input.Headers;
        subscription.SetRetryPolicy(input.MaxRetryCount, input.TimeoutSeconds);
        subscription.Description = input.Description;

        await _subscriptionRepository.InsertAsync(subscription, autoSave: true);

        return ObjectMapper.Map<WebhookSubscription, WebhookSubscriptionDto>(subscription);
    }

    public async Task<WebhookSubscriptionDto> UpdateAsync(Guid id, UpdateWebhookSubscriptionDto input)
    {
        var subscription = await _subscriptionRepository.GetAsync(id);

        subscription.UpdateSubscription(
            input.Name,
            input.Url,
            input.SubscribedEvents,
            input.IsActive
        );

        subscription.Headers = input.Headers;
        subscription.SetRetryPolicy(input.MaxRetryCount, input.TimeoutSeconds);
        subscription.Description = input.Description;

        await _subscriptionRepository.UpdateAsync(subscription, autoSave: true);

        return ObjectMapper.Map<WebhookSubscription, WebhookSubscriptionDto>(subscription);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _subscriptionRepository.DeleteAsync(id);
    }

    public async Task<WebhookSubscriptionDto> ActivateAsync(Guid id)
    {
        var subscription = await _subscriptionRepository.GetAsync(id);
        subscription.UpdateSubscription(
            subscription.Name,
            subscription.Url,
            subscription.SubscribedEvents,
            true
        );
        await _subscriptionRepository.UpdateAsync(subscription, autoSave: true);
        return ObjectMapper.Map<WebhookSubscription, WebhookSubscriptionDto>(subscription);
    }

    public async Task<WebhookSubscriptionDto> DeactivateAsync(Guid id)
    {
        var subscription = await _subscriptionRepository.GetAsync(id);
        subscription.UpdateSubscription(
            subscription.Name,
            subscription.Url,
            subscription.SubscribedEvents,
            false
        );
        await _subscriptionRepository.UpdateAsync(subscription, autoSave: true);
        return ObjectMapper.Map<WebhookSubscription, WebhookSubscriptionDto>(subscription);
    }

    public Task<List<string>> GetAvailableEventTypesAsync()
    {
        var eventTypes = new List<string>
        {
            "EmailManagement.MailMessage.Received",
            "EmailManagement.MailMessage.Sent",
            "EmailManagement.MailSendTask.Created",
            "EmailManagement.MailSendTask.Failed",
            "EmailManagement.MailThread.StatusChanged",
            "EmailManagement.MailApproval.Requested",
            "EmailManagement.MailApproval.Completed",
            "EmailManagement.MailAttachment.Accessed",
            "EmailManagement.MailRule.Executed"
        };

        return Task.FromResult(eventTypes);
    }
}
