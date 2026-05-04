using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Domain.Webhook;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories.Webhook;

public class WebhookDeliveryLogRepository :
    EfCoreRepository<EmailManagementDbContext, WebhookDeliveryLog, Guid>,
    IWebhookDeliveryLogRepository
{
    public WebhookDeliveryLogRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<WebhookDeliveryLog>> GetPendingDeliveriesAsync(
        int maxCount = 100,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.DeliveryStatus == WebhookDeliveryStatus.Pending)
            .OrderBy(x => x.CreationTime)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WebhookDeliveryLog>> GetFailedDeliveriesForRetryAsync(
        DateTime beforeTime,
        int maxCount = 100,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.DeliveryStatus == WebhookDeliveryStatus.Failed &&
                       x.NextRetryTime != null &&
                       x.NextRetryTime <= beforeTime)
            .OrderBy(x => x.NextRetryTime)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }
}
