using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Domain.Webhook;

/// <summary>
/// Webhook订阅仓储接口
/// </summary>
public interface IWebhookSubscriptionRepository : IRepository<WebhookSubscription, Guid>
{
    Task<List<WebhookSubscription>> GetActiveSubscriptionsByEventTypeAsync(
        string eventType,
        CancellationToken cancellationToken = default
    );

    Task<List<WebhookSubscription>> GetAllActiveAsync(
        CancellationToken cancellationToken = default
    );
}
