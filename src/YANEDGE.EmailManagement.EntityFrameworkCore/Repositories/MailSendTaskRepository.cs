using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

public class MailSendTaskRepository : EfCoreRepository<EmailManagementDbContext, MailSendTask, Guid>, IMailSendTaskRepository
{
    public MailSendTaskRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<MailSendTask>> GetPendingSendTasksAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Status == SendTaskStatus.PendingSend)
            .OrderBy(x => x.ScheduledSendTime ?? x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailSendTask>> GetFailedTasksForRetryAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Status == SendTaskStatus.Failed && x.RetryCount < x.MaxRetryCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<MailSendTask?> FindByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    public async Task<MailSendTask?> FindByExternalBizRefAsync(
        string externalBizRef,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.ExternalBizRef == externalBizRef, cancellationToken);
    }

    public async Task<List<MailSendTask>> GetByStatusAsync(
        SendTaskStatus status,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.Status == status).ToListAsync(cancellationToken);
    }
}
