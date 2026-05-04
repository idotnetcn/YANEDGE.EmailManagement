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

public class WebhookSubscriptionRepository :
    EfCoreRepository<EmailManagementDbContext, WebhookSubscription, Guid>,
    IWebhookSubscriptionRepository
{
    public WebhookSubscriptionRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<WebhookSubscription>> GetActiveSubscriptionsByEventTypeAsync(
        string eventType,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.IsActive && x.SubscribedEvents.Contains(eventType))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WebhookSubscription>> GetAllActiveAsync(
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);
    }
}
