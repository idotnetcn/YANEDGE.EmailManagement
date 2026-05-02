using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Domain.Collaboration;

/// <summary>
/// 内部备注仓储接口
/// </summary>
public interface IInternalNoteRepository : IRepository<InternalNote, Guid>
{
    /// <summary>
    /// 根据线程ID获取备注列表
    /// </summary>
    Task<List<InternalNote>> GetByThreadIdAsync(Guid threadId);

    /// <summary>
    /// 根据消息ID获取备注列表
    /// </summary>
    Task<List<InternalNote>> GetByMessageIdAsync(Guid messageId);
}
