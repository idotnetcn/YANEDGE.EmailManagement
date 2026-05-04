using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Domain.MailThread;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

public class MailThreadRepository : EfCoreRepository<EmailManagementDbContext, MailThread, Guid>, IMailThreadRepository
{
    public MailThreadRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<MailThread>> GetByAssigneeAsync(
        AssigneeType assigneeType,
        Guid assigneeId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.CurrentAssigneeType == assigneeType && x.CurrentAssigneeId == assigneeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailThread>> GetByStatusAsync(
        ThreadStatus status,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.Status == status).ToListAsync(cancellationToken);
    }

    public async Task<List<MailThread>> FindCandidateThreadsForMergeAsync(
        Guid mailAccountId,
        string normalizedSubject,
        DateTime timeWindowStart,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x =>
                x.MailAccountId == mailAccountId &&
                x.NormalizedSubject == normalizedSubject &&
                x.LatestMessageTime >= timeWindowStart)
            .OrderByDescending(x => x.LatestMessageTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<MailThread> threads, long totalCount)> GetPagedListAsync(
        Guid? mailAccountId = null,
        ThreadStatus? status = null,
        Guid? assigneeId = null,
        bool? hasAttachment = null,
        int skipCount = 0,
        int maxResultCount = 20,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking(); // Performance optimization: no change tracking for read-only queries

        // Apply filters
        if (mailAccountId.HasValue)
        {
            query = query.Where(x => x.MailAccountId == mailAccountId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (assigneeId.HasValue)
        {
            query = query.Where(x => x.CurrentAssigneeId == assigneeId.Value);
        }

        if (hasAttachment.HasValue)
        {
            query = query.Where(x => x.HasAttachment == hasAttachment.Value);
        }

        // Get total count
        var totalCount = await query.LongCountAsync(cancellationToken);

        // Apply pagination and sorting
        var threads = await query
            .OrderByDescending(x => x.LatestMessageTime)
            .ThenByDescending(x => x.Id) // Secondary sort for stable pagination
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);

        return (threads, totalCount);
    }

    public async Task<MailThread?> GetWithoutTrackingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
