using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Domain.Collaboration;

/// <summary>
/// 内部备注
/// </summary>
public class InternalNote : Entity<Guid>
{
    /// <summary>
    /// 线程ID
    /// </summary>
    public Guid ThreadId { get; private set; }

    /// <summary>
    /// 备注内容
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    public bool IsPinned { get; private set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    public Guid CreatedByUserId { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    private InternalNote()
    {
        // For ORM
        Content = string.Empty;
    }

    public InternalNote(
        Guid id,
        Guid threadId,
        string content,
        Guid createdByUserId,
        bool isPinned = false) : base(id)
    {
        ThreadId = threadId;
        Content = content;
        CreatedByUserId = createdByUserId;
        IsPinned = isPinned;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string content)
    {
        Content = content;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Pin()
    {
        IsPinned = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unpin()
    {
        IsPinned = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
