using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Domain.Collaboration;

/// <summary>
/// 线程分派记录仓储接口
/// </summary>
public interface IThreadAssignmentRepository : IRepository<ThreadAssignment, Guid>
{
    /// <summary>
    /// 根据线程ID获取分派记录列表
    /// </summary>
    Task<List<ThreadAssignment>> GetByThreadIdAsync(Guid threadId);

    /// <summary>
    /// 获取用户的分派记录
    /// </summary>
    Task<List<ThreadAssignment>> GetByAssigneeAsync(Guid assigneeId);

    /// <summary>
    /// 获取最新的生效分派记录
    /// </summary>
    Task<ThreadAssignment?> GetActiveAssignmentAsync(Guid threadId);
}
