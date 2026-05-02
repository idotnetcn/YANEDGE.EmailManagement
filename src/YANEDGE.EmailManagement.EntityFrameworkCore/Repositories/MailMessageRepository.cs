using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Domain.MailMessage;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

public class MailMessageRepository : EfCoreRepository<EmailManagementDbContext, MailMessage, Guid>, IMailMessageRepository
{
    public MailMessageRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<MailMessage?> FindByInternetMessageIdAsync(
        string internetMessageId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.InternetMessageId == internetMessageId, cancellationToken);
    }

    public async Task<List<MailMessage>> GetByThreadIdAsync(
        Guid threadId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.ThreadId == threadId)
            .OrderBy(x => x.ReceivedTime ?? x.SentTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailMessage>> GetByMailAccountIdAsync(
        Guid mailAccountId,
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.MailAccountId == mailAccountId)
            .OrderByDescending(x => x.ReceivedTime ?? x.SentTime)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }
}
