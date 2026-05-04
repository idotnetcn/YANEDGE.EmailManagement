using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Webhook;

/// <summary>
/// Webhook订阅应用服务接口
/// </summary>
public interface IWebhookSubscriptionAppService : IApplicationService
{
    Task<WebhookSubscriptionDto> GetAsync(Guid id);

    Task<PagedResultDto<WebhookSubscriptionDto>> GetListAsync(WebhookSubscriptionGetListInput input);

    Task<WebhookSubscriptionDto> CreateAsync(CreateWebhookSubscriptionDto input);

    Task<WebhookSubscriptionDto> UpdateAsync(Guid id, UpdateWebhookSubscriptionDto input);

    Task DeleteAsync(Guid id);

    Task<WebhookSubscriptionDto> ActivateAsync(Guid id);

    Task<WebhookSubscriptionDto> DeactivateAsync(Guid id);

    Task<List<string>> GetAvailableEventTypesAsync();
}

public class WebhookSubscriptionDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<string> SubscribedEvents { get; set; } = new();
    public Dictionary<string, string>? Headers { get; set; }
    public int MaxRetryCount { get; set; }
    public int TimeoutSeconds { get; set; }
    public string? Description { get; set; }
    public DateTime CreationTime { get; set; }
}

public class CreateWebhookSubscriptionDto
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public List<string> SubscribedEvents { get; set; } = new();
    public Dictionary<string, string>? Headers { get; set; }
    public int MaxRetryCount { get; set; } = 5;
    public int TimeoutSeconds { get; set; } = 30;
    public string? Description { get; set; }
}

public class UpdateWebhookSubscriptionDto
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public List<string> SubscribedEvents { get; set; } = new();
    public bool IsActive { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
    public int MaxRetryCount { get; set; } = 5;
    public int TimeoutSeconds { get; set; } = 30;
    public string? Description { get; set; }
}

public class WebhookSubscriptionGetListInput : PagedAndSortedResultRequestDto
{
    public bool? IsActive { get; set; }
    public string? Filter { get; set; }
}
