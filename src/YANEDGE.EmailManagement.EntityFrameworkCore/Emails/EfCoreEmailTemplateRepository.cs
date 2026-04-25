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

public class EfCoreEmailTemplateRepository : EfCoreRepository<EmailManagementDbContext, EmailTemplate, Guid>, IEmailTemplateRepository
{
    public EfCoreEmailTemplateRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<EmailTemplate?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.EmailTemplates
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<List<EmailTemplate>> GetListAsync(
        bool? isActive = null,
        string? filter = null,
        int maxResultCount = 10,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.EmailTemplates
            .WhereIf(isActive.HasValue, t => t.IsActive == isActive!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(filter),
                t => t.Name.Contains(filter!) || t.Subject.Contains(filter!))
            .OrderBy(t => t.Name)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }
}
