using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Domain.Template;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

/// <summary>
/// 邮件签名仓储实现
/// </summary>
public class MailSignatureRepository : EfCoreRepository<EmailManagementDbContext, MailSignature, Guid>, IMailSignatureRepository
{
    public MailSignatureRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<MailSignature>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Scope == SignatureScope.Personal && x.OwnerUserId == userId && x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailSignature>> GetByOrganizationIdAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Scope == SignatureScope.Department && x.OwnerOrganizationId == organizationId && x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailSignature>> GetGlobalSignaturesAsync(
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Scope == SignatureScope.Global && x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<MailSignature?> GetDefaultSignatureByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.Scope == SignatureScope.Personal && x.OwnerUserId == userId && x.IsDefault && x.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
