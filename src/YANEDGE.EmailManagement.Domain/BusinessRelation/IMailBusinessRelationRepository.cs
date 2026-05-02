using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.BusinessRelation;

/// <summary>
/// 邮件业务对象关联仓储接口
/// </summary>
public interface IMailBusinessRelationRepository : IRepository<MailBusinessRelation, Guid>
{
    /// <summary>
    /// 获取邮件的业务对象关联列表
    /// </summary>
    Task<List<MailBusinessRelation>> GetByMailMessageIdAsync(
        Guid mailMessageId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取线程的业务对象关联列表
    /// </summary>
    Task<List<MailBusinessRelation>> GetByThreadIdAsync(
        Guid threadId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取指定业务对象的邮件关联列表
    /// </summary>
    Task<List<MailBusinessRelation>> GetByBusinessObjectAsync(
        BusinessObjectType businessObjectType,
        string businessObjectId,
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 查找主要业务对象关联
    /// </summary>
    Task<MailBusinessRelation?> FindPrimaryRelationByMailMessageIdAsync(
        Guid mailMessageId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 检查关联是否存在
    /// </summary>
    Task<bool> ExistsAsync(
        Guid mailMessageId,
        BusinessObjectType businessObjectType,
        string businessObjectId,
        CancellationToken cancellationToken = default
    );
}
