namespace YANEDGE.EmailManagement.Services.Cache;

/// <summary>
/// User workbench cache service
/// Caches user's workbench statistics and todo counts
/// </summary>
public interface IUserWorkbenchCacheService
{
    /// <summary>
    /// Get user's todo count from cache
    /// </summary>
    Task<int?> GetTodoCountAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set user's todo count cache
    /// </summary>
    Task SetTodoCountAsync(Guid userId, int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove user's todo count cache
    /// </summary>
    Task RemoveTodoCountAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get workbench metrics from cache
    /// </summary>
    Task<WorkbenchMetricsCache?> GetWorkbenchMetricsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set workbench metrics cache
    /// </summary>
    Task SetWorkbenchMetricsAsync(WorkbenchMetricsCache metrics, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove workbench metrics cache
    /// </summary>
    Task RemoveWorkbenchMetricsAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Workbench metrics cache model
/// </summary>
public class WorkbenchMetricsCache
{
    public Guid UserId { get; set; }
    public int TodoCount { get; set; }
    public int UnreadCount { get; set; }
    public int PendingApprovalCount { get; set; }
    public int FailedSendTaskCount { get; set; }
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;
}
