using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace YANEDGE.EmailManagement.Services.Cache.Implementation;

/// <summary>
/// User workbench cache service implementation
/// TTL: 15-60 seconds for todo counts, 30s-5min for metrics based on design document
/// </summary>
public class UserWorkbenchCacheService : IUserWorkbenchCacheService, ITransientDependency
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<UserWorkbenchCacheService> _logger;
    private static readonly TimeSpan TodoCountTtl = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan MetricsTtl = TimeSpan.FromMinutes(2);

    public UserWorkbenchCacheService(
        ICacheService cacheService,
        ILogger<UserWorkbenchCacheService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<int?> GetTodoCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = GetTodoCountKey(userId);
        var cached = await _cacheService.GetAsync<TodoCountCache>(key, cancellationToken);
        return cached?.Count;
    }

    public async Task SetTodoCountAsync(Guid userId, int count, CancellationToken cancellationToken = default)
    {
        var key = GetTodoCountKey(userId);
        var cache = new TodoCountCache { UserId = userId, Count = count };
        await _cacheService.SetAsync(key, cache, TodoCountTtl, cancellationToken);
        _logger.LogDebug("User todo count cached: {UserId}, Count: {Count}", userId, count);
    }

    public async Task RemoveTodoCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = GetTodoCountKey(userId);
        await _cacheService.RemoveAsync(key, cancellationToken);
        _logger.LogDebug("User todo count cache removed: {UserId}", userId);
    }

    public async Task<WorkbenchMetricsCache?> GetWorkbenchMetricsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = GetMetricsKey(userId);
        return await _cacheService.GetAsync<WorkbenchMetricsCache>(key, cancellationToken);
    }

    public async Task SetWorkbenchMetricsAsync(WorkbenchMetricsCache metrics, CancellationToken cancellationToken = default)
    {
        var key = GetMetricsKey(metrics.UserId);
        await _cacheService.SetAsync(key, metrics, MetricsTtl, cancellationToken);
        _logger.LogDebug("User workbench metrics cached: {UserId}", metrics.UserId);
    }

    public async Task RemoveWorkbenchMetricsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = GetMetricsKey(userId);
        await _cacheService.RemoveAsync(key, cancellationToken);
        _logger.LogDebug("User workbench metrics cache removed: {UserId}", userId);
    }

    private static string GetTodoCountKey(Guid userId)
    {
        return $"user:todo-count:{userId}";
    }

    private static string GetMetricsKey(Guid userId)
    {
        return $"user:workbench:{userId}";
    }

    private class TodoCountCache
    {
        public Guid UserId { get; set; }
        public int Count { get; set; }
    }
}
