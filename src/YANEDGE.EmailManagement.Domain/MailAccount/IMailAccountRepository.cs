using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Domain.MailAccount;

/// <summary>
/// 邮箱账号仓储接口
/// </summary>
public interface IMailAccountRepository : IRepository<MailAccount, Guid>
{
    /// <summary>
    /// 根据邮箱地址查询
    /// </summary>
    Task<MailAccount?> FindByEmailAddressAsync(string emailAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询启用同步的账号
    /// </summary>
    Task<List<MailAccount>> GetSyncEnabledAccountsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询启用发件的账号
    /// </summary>
    Task<List<MailAccount>> GetSendEnabledAccountsAsync(CancellationToken cancellationToken = default);
}
