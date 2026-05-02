using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Attachment;

/// <summary>
/// 邮件附件仓储接口
/// </summary>
public interface IMailAttachmentRepository : IRepository<MailAttachment, Guid>
{
    /// <summary>
    /// 获取指定邮件的附件列表
    /// </summary>
    Task<List<MailAttachment>> GetByMailMessageIdAsync(
        Guid mailMessageId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 根据文件哈希查找附件
    /// </summary>
    Task<List<MailAttachment>> FindByFileHashAsync(
        string fileHash,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取敏感附件列表
    /// </summary>
    Task<List<MailAttachment>> GetSensitiveAttachmentsAsync(
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取未扫描的附件列表
    /// </summary>
    Task<List<MailAttachment>> GetUnscannedAttachmentsAsync(
        int maxResultCount,
        CancellationToken cancellationToken = default
    );
}
