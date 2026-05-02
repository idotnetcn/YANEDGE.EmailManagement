using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Template;

/// <summary>
/// 邮件签名仓储接口
/// </summary>
public interface IMailSignatureRepository : IRepository<MailSignature, Guid>
{
    /// <summary>
    /// 获取用户的签名列表
    /// </summary>
    Task<List<MailSignature>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取组织的签名列表
    /// </summary>
    Task<List<MailSignature>> GetByOrganizationIdAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取全局签名列表
    /// </summary>
    Task<List<MailSignature>> GetGlobalSignaturesAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取用户的默认签名
    /// </summary>
    Task<MailSignature?> GetDefaultSignatureByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );
}
