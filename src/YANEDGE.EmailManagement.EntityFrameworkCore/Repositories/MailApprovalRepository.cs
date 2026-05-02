using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Domain.Approval;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

public class MailApprovalRepository : EfCoreRepository<EmailManagementDbContext, MailApproval, Guid>, IMailApprovalRepository
{
    public MailApprovalRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<MailApproval>> GetPendingApprovalsAsync(
        Guid approverId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Status == ApprovalStatus.Pending && x.CurrentApproverId == approverId)
            .OrderBy(x => x.SubmittedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<MailApproval?> FindByBusinessIdAsync(
        string businessType,
        Guid businessId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(x => x.BusinessType == businessType && x.BusinessId == businessId, cancellationToken);
    }

    public async Task<List<MailApproval>> GetByStatusAsync(
        ApprovalStatus status,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.Status == status).ToListAsync(cancellationToken);
    }
}
