using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Rule;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

/// <summary>
/// 邮件规则仓储实现
/// </summary>
public class MailRuleRepository : EfCoreRepository<EmailManagementDbContext, MailRule, Guid>, IMailRuleRepository
{
    public MailRuleRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<MailRule>> GetActiveRulesOrderedByPriorityAsync(
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.Priority)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailRule>> GetApplicableRulesForMailAccountAsync(
        Guid mailAccountId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.IsActive)
            .Where(x => x.ApplicableMailAccountIds.Count == 0 || x.ApplicableMailAccountIds.Contains(mailAccountId))
            .OrderByDescending(x => x.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<MailRule?> FindByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
}
