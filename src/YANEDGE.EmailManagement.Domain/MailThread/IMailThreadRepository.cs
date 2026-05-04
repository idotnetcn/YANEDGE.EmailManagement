using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.MailThread;

/// <summary>
/// 邮件线程仓储接口
/// </summary>
public interface IMailThreadRepository : IRepository<MailThread, Guid>
{
    /// <summary>
    /// 按负责人查询线程
    /// </summary>
    Task<List<MailThread>> GetByAssigneeAsync(
        AssigneeType assigneeType,
        Guid assigneeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按状态查询线程
    /// </summary>
    Task<List<MailThread>> GetByStatusAsync(
        ThreadStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按主题和时间窗口查找候选线程(用于归并)
    /// </summary>
    Task<List<MailThread>> FindCandidateThreadsForMergeAsync(
        Guid mailAccountId,
        string normalizedSubject,
        DateTime timeWindowStart,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paginated thread list with optimized projection (no tracking)
    /// Returns lightweight DTOs for list views
    /// </summary>
    Task<(List<MailThread> threads, long totalCount)> GetPagedListAsync(
        Guid? mailAccountId = null,
        ThreadStatus? status = null,
        Guid? assigneeId = null,
        bool? hasAttachment = null,
        int skipCount = 0,
        int maxResultCount = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get thread by ID with no tracking for read-only scenarios
    /// </summary>
    Task<MailThread?> GetWithoutTrackingAsync(Guid id, CancellationToken cancellationToken = default);
}
