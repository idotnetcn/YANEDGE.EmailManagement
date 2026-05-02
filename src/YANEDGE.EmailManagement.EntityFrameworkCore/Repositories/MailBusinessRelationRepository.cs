using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Domain.BusinessRelation;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

/// <summary>
/// 邮件业务对象关联仓储实现
/// </summary>
public class MailBusinessRelationRepository : EfCoreRepository<EmailManagementDbContext, MailBusinessRelation, Guid>, IMailBusinessRelationRepository
{
    public MailBusinessRelationRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<MailBusinessRelation>> GetByMailMessageIdAsync(
        Guid mailMessageId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.MailMessageId == mailMessageId)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.BusinessObjectType)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailBusinessRelation>> GetByThreadIdAsync(
        Guid threadId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.ThreadId == threadId)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.BusinessObjectType)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailBusinessRelation>> GetByBusinessObjectAsync(
        BusinessObjectType businessObjectType,
        string businessObjectId,
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.BusinessObjectType == businessObjectType && x.BusinessObjectId == businessObjectId)
            .OrderByDescending(x => x.CreationTime)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<MailBusinessRelation?> FindPrimaryRelationByMailMessageIdAsync(
        Guid mailMessageId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.MailMessageId == mailMessageId && x.IsPrimary)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid mailMessageId,
        BusinessObjectType businessObjectType,
        string businessObjectId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AnyAsync(x => x.MailMessageId == mailMessageId &&
                          x.BusinessObjectType == businessObjectType &&
                          x.BusinessObjectId == businessObjectId,
                     cancellationToken);
    }
}
