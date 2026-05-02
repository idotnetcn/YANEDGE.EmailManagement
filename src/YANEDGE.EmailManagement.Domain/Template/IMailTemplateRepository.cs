using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Template;

/// <summary>
/// 邮件模板仓储接口
/// </summary>
public interface IMailTemplateRepository : IRepository<MailTemplate, Guid>
{
    /// <summary>
    /// 根据编码查找模板
    /// </summary>
    Task<MailTemplate?> FindByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取指定分类的模板列表
    /// </summary>
    Task<List<MailTemplate>> GetByCategoryAsync(
        string category,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取所有启用的模板
    /// </summary>
    Task<List<MailTemplate>> GetActiveTemplatesAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取默认模板
    /// </summary>
    Task<List<MailTemplate>> GetDefaultTemplatesAsync(
        CancellationToken cancellationToken = default
    );
}
