using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace YANEDGE.EmailManagement.Services.Cache.Implementation;

/// <summary>
/// Thread summary cache service implementation
/// TTL: 30 seconds to 5 minutes based on design document
/// </summary>
public class ThreadSummaryCacheService : IThreadSummaryCacheService, ITransientDependency
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<ThreadSummaryCacheService> _logger;
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(3);

    public ThreadSummaryCacheService(
        ICacheService cacheService,
        ILogger<ThreadSummaryCacheService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ThreadSummaryCache?> GetThreadSummaryAsync(Guid threadId, CancellationToken cancellationToken = default)
    {
        var key = GetCacheKey(threadId);
        return await _cacheService.GetAsync<ThreadSummaryCache>(key, cancellationToken);
    }

    public async Task SetThreadSummaryAsync(ThreadSummaryCache summary, CancellationToken cancellationToken = default)
    {
        var key = GetCacheKey(summary.ThreadId);
        await _cacheService.SetAsync(key, summary, DefaultTtl, cancellationToken);
        _logger.LogDebug("Thread summary cached: {ThreadId}", summary.ThreadId);
    }

    public async Task RemoveThreadSummaryAsync(Guid threadId, CancellationToken cancellationToken = default)
    {
        var key = GetCacheKey(threadId);
        await _cacheService.RemoveAsync(key, cancellationToken);
        _logger.LogDebug("Thread summary cache removed: {ThreadId}", threadId);
    }

    public async Task<ThreadSummaryCache> GetOrLoadThreadSummaryAsync(
        Guid threadId,
        Func<Task<ThreadSummaryCache>> factory,
        CancellationToken cancellationToken = default)
    {
        var key = GetCacheKey(threadId);
        return await _cacheService.GetOrSetAsync(key, factory, DefaultTtl, cancellationToken);
    }

    private static string GetCacheKey(Guid threadId)
    {
        return $"thread:summary:{threadId}";
    }
}
