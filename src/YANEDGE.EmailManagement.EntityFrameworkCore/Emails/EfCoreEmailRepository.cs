using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Emails;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Emails;

public class EfCoreEmailRepository : EfCoreRepository<EmailManagementDbContext, Email, Guid>, IEmailRepository
{
    public EfCoreEmailRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Email>> GetListAsync(
        EmailStatus? status = null,
        EmailPriority? priority = null,
        string? fromAddress = null,
        DateTime? scheduledBefore = null,
        int maxResultCount = 10,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Emails
            .Include(e => e.Recipients)
            .Include(e => e.Attachments)
            .WhereIf(status.HasValue, e => e.Status == status!.Value)
            .WhereIf(priority.HasValue, e => e.Priority == priority!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(fromAddress), e => e.FromAddress.Contains(fromAddress!))
            .WhereIf(scheduledBefore.HasValue, e => e.ScheduledAt <= scheduledBefore!.Value)
            .OrderByDescending(e => e.CreationTime)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> GetCountAsync(
        EmailStatus? status = null,
        EmailPriority? priority = null,
        string? fromAddress = null,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Emails
            .WhereIf(status.HasValue, e => e.Status == status!.Value)
            .WhereIf(priority.HasValue, e => e.Priority == priority!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(fromAddress), e => e.FromAddress.Contains(fromAddress!))
            .LongCountAsync(cancellationToken);
    }

    public async Task<List<Email>> GetQueuedEmailsAsync(
        int maxCount = 50,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Emails
            .Include(e => e.Recipients)
            .Include(e => e.Attachments)
            .Where(e => e.Status == EmailStatus.Queued &&
                        (e.ScheduledAt == null || e.ScheduledAt <= DateTime.UtcNow))
            .OrderBy(e => e.Priority)
            .ThenBy(e => e.CreationTime)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }
}
