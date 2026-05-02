using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Domain.MailAccount;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

public class MailAccountRepository : EfCoreRepository<EmailManagementDbContext, MailAccount, Guid>, IMailAccountRepository
{
    public MailAccountRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<MailAccount?> FindByEmailAddressAsync(string emailAddress, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.EmailAddress == emailAddress, cancellationToken);
    }

    public async Task<List<MailAccount>> GetSyncEnabledAccountsAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.SyncEnabled).ToListAsync(cancellationToken);
    }

    public async Task<List<MailAccount>> GetSendEnabledAccountsAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.SendEnabled).ToListAsync(cancellationToken);
    }
}
