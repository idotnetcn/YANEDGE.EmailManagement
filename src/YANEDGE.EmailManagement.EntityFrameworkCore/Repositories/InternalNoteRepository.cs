using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Domain.Collaboration;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

public class InternalNoteRepository : EfCoreRepository<EmailManagementDbContext, InternalNote, Guid>, IInternalNoteRepository
{
    public InternalNoteRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<InternalNote>> GetByThreadIdAsync(Guid threadId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(n => n.ThreadId == threadId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<InternalNote>> GetByMessageIdAsync(Guid messageId)
    {
        // InternalNote doesn't have MessageId field in current model
        // This is reserved for future enhancement when we add MessageId to the entity
        // For now, return empty list
        await Task.CompletedTask;
        return new List<InternalNote>();
    }
}
