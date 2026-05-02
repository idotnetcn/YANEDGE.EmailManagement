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
}
