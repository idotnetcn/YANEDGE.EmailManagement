using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Domain.Collaboration;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

public class ThreadAssignmentRepository : EfCoreRepository<EmailManagementDbContext, ThreadAssignment, Guid>, IThreadAssignmentRepository
{
    public ThreadAssignmentRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<ThreadAssignment>> GetByThreadIdAsync(Guid threadId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(a => a.ThreadId == threadId)
            .OrderByDescending(a => a.AssignedAt)
            .ToListAsync();
    }

    public async Task<List<ThreadAssignment>> GetByAssigneeAsync(Guid assigneeId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(a => a.ToAssigneeId == assigneeId)
            .OrderByDescending(a => a.AssignedAt)
            .ToListAsync();
    }

    public async Task<ThreadAssignment?> GetActiveAssignmentAsync(Guid threadId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(a => a.ThreadId == threadId)
            .OrderByDescending(a => a.AssignedAt)
            .FirstOrDefaultAsync();
    }
}
