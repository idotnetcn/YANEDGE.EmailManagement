using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Attachment;

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Repositories;

/// <summary>
/// 邮件附件仓储实现
/// </summary>
public class MailAttachmentRepository : EfCoreRepository<EmailManagementDbContext, MailAttachment, Guid>, IMailAttachmentRepository
{
    public MailAttachmentRepository(IDbContextProvider<EmailManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<MailAttachment>> GetByMailMessageIdAsync(
        Guid mailMessageId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.MailMessageId == mailMessageId)
            .OrderBy(x => x.IsInline)
            .ThenBy(x => x.FileName)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailAttachment>> FindByFileHashAsync(
        string fileHash,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.FileHash == fileHash)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailAttachment>> GetSensitiveAttachmentsAsync(
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.IsSensitive)
            .OrderByDescending(x => x.CreationTime)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MailAttachment>> GetUnscannedAttachmentsAsync(
        int maxResultCount,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => !x.IsScanned)
            .OrderBy(x => x.CreationTime)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }
}
