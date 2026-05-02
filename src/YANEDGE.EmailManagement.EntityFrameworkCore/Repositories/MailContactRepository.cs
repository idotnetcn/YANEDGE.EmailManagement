using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Contact;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

/// <summary>
/// 联系人仓储实现
/// </summary>
public class MailContactRepository : EfCoreRepository<EmailManagementDbContext, MailContact, Guid>, IMailContactRepository
{
    public MailContactRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<MailContact?> FindByEmailAddressAsync(
        string emailAddress,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.EmailAddress == emailAddress && x.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<MailContact>> GetByCustomerIdAsync(
        string customerId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.CustomerId == customerId && x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailContact>> GetBySupplierIdAsync(
        string supplierId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.SupplierId == supplierId && x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<MailContact?> FindByExternalIdAsync(
        string source,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Source == source && x.ExternalId == externalId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<MailContact>> SearchAsync(
        string keyword,
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.IsActive)
            .Where(x => x.Name.Contains(keyword) ||
                       x.EmailAddress.Contains(keyword) ||
                       (x.CompanyName != null && x.CompanyName.Contains(keyword)))
            .OrderBy(x => x.Name)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }
}
