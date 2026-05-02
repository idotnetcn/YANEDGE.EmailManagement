using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Domain.MailMessage;

/// <summary>
/// 邮件消息仓储接口
/// </summary>
public interface IMailMessageRepository : IRepository<MailMessage, Guid>
{
    /// <summary>
    /// 按InternetMessageId查询
    /// </summary>
    Task<MailMessage?> FindByInternetMessageIdAsync(
        string internetMessageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按线程ID查询邮件列表
    /// </summary>
    Task<List<MailMessage>> GetByThreadIdAsync(
        Guid threadId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按邮箱账号查询邮件列表
    /// </summary>
    Task<List<MailMessage>> GetByMailAccountIdAsync(
        Guid mailAccountId,
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default);
}
