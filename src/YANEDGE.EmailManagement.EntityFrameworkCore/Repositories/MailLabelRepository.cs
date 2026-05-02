using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Domain.Label;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

/// <summary>
/// 邮件标签仓储实现
/// </summary>
public class MailLabelRepository : EfCoreRepository<EmailManagementDbContext, MailLabel, Guid>, IMailLabelRepository
{
    public MailLabelRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<MailLabel?> FindByNameAsync(
        string name,
        Guid? ownerUserId = null,
        Guid? ownerOrganizationId = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Name == name)
            .Where(x => x.OwnerUserId == ownerUserId && x.OwnerOrganizationId == ownerOrganizationId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<MailLabel>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.OwnerUserId == userId && x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailLabel>> GetByOrganizationIdAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.OwnerOrganizationId == organizationId && x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailLabel>> GetSystemLabelsAsync(
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.IsSystemLabel && x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
