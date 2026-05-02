using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.MailCompose;

/// <summary>
/// 发件任务仓储接口
/// </summary>
public interface IMailSendTaskRepository : IRepository<MailSendTask, Guid>
{
    /// <summary>
    /// 查询待发送任务
    /// </summary>
    Task<List<MailSendTask>> GetPendingSendTasksAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询失败待重试任务
    /// </summary>
    Task<List<MailSendTask>> GetFailedTasksForRetryAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 按幂等键查询
    /// </summary>
    Task<MailSendTask?> FindByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按外部业务引用查询
    /// </summary>
    Task<MailSendTask?> FindByExternalBizRefAsync(
        string externalBizRef,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按审批状态查询
    /// </summary>
    Task<List<MailSendTask>> GetByStatusAsync(
        SendTaskStatus status,
        CancellationToken cancellationToken = default);
}
