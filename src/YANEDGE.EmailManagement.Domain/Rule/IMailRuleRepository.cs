using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Domain.Rule;

/// <summary>
/// 邮件规则仓储接口
/// </summary>
public interface IMailRuleRepository : IRepository<MailRule, Guid>
{
    /// <summary>
    /// 获取启用的规则列表 (按优先级排序)
    /// </summary>
    Task<List<MailRule>> GetActiveRulesOrderedByPriorityAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 获取适用于指定邮箱的规则列表
    /// </summary>
    Task<List<MailRule>> GetApplicableRulesForMailAccountAsync(
        Guid mailAccountId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 根据名称查找规则
    /// </summary>
    Task<MailRule?> FindByNameAsync(
        string name,
        CancellationToken cancellationToken = default
    );
}
