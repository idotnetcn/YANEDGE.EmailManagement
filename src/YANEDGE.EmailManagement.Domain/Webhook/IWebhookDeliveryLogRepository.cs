using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Task;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Domain.Webhook;

/// <summary>
/// Webhook投递日志仓储接口
/// </summary>
public interface IWebhookDeliveryLogRepository : IRepository<WebhookDeliveryLog, Guid>
{
    Task<List<WebhookDeliveryLog>> GetPendingDeliveriesAsync(
        int maxCount = 100,
        CancellationToken cancellationToken = default
    );

    Task<List<WebhookDeliveryLog>> GetFailedDeliveriesForRetryAsync(
        DateTime beforeTime,
        int maxCount = 100,
        CancellationToken cancellationToken = default
    );
}
