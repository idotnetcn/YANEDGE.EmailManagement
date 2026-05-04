using YANEDGE.EmailManagement.Domain.MailThread;

namespace YANEDGE.EmailManagement.Services.Cache;

/// <summary>
/// Thread summary cache service
/// Caches frequently accessed thread summary data
/// </summary>
public interface IThreadSummaryCacheService
{
    /// <summary>
    /// Get thread summary from cache
    /// </summary>
    Task<ThreadSummaryCache?> GetThreadSummaryAsync(Guid threadId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set thread summary cache
    /// </summary>
    Task SetThreadSummaryAsync(ThreadSummaryCache summary, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove thread summary cache
    /// </summary>
    Task RemoveThreadSummaryAsync(Guid threadId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get or load thread summary from cache
    /// </summary>
    Task<ThreadSummaryCache> GetOrLoadThreadSummaryAsync(
        Guid threadId,
        Func<Task<ThreadSummaryCache>> factory,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Thread summary cache model
/// Lightweight DTO for caching thread information
/// </summary>
public class ThreadSummaryCache
{
    public Guid ThreadId { get; set; }
    public Guid MailAccountId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public int ThreadStatus { get; set; }
    public DateTime LatestMessageTime { get; set; }
    public int UnreadCount { get; set; }
    public int MessageCount { get; set; }
    public bool HasAttachment { get; set; }
    public int? Priority { get; set; }
    public Guid? CurrentAssigneeId { get; set; }
    public string? CurrentAssigneeName { get; set; }
    public int Version { get; set; }
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;
}
