using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Template;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

/// <summary>
/// 邮件模板仓储实现
/// </summary>
public class MailTemplateRepository : EfCoreRepository<EmailManagementDbContext, MailTemplate, Guid>, IMailTemplateRepository
{
    public MailTemplateRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<MailTemplate?> FindByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<List<MailTemplate>> GetByCategoryAsync(
        string category,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Category == category)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailTemplate>> GetActiveTemplatesAsync(
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Status == Enums.TemplateStatus.Active)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailTemplate>> GetDefaultTemplatesAsync(
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.IsDefault)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);
    }
}
