using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Domain.Label;

/// <summary>
/// 邮件标签仓储接口
/// </summary>
public interface IMailLabelRepository : IRepository<MailLabel, Guid>
{
    /// <summary>
    /// 根据名称查找标签
    /// </summary>
    Task<MailLabel?> FindByNameAsync(
        string name,
        Guid? ownerUserId = null,
        Guid? ownerOrganizationId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取用户的标签列表
    /// </summary>
    Task<List<MailLabel>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取组织的标签列表
    /// </summary>
    Task<List<MailLabel>> GetByOrganizationIdAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取系统标签列表
    /// </summary>
    Task<List<MailLabel>> GetSystemLabelsAsync(
        CancellationToken cancellationToken = default
    );
}
