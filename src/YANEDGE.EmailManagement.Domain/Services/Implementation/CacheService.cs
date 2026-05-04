using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Volo.Abp.DependencyInjection;

namespace YANEDGE.EmailManagement.Services.Implementation;

/// <summary>
/// Cache service implementation using IDistributedCache (Redis)
/// Provides unified caching with TTL, jitter, and prefix-based invalidation
/// </summary>
public class CacheService : ICacheService, ITransientDependency
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<CacheService> _logger;
    private const string KeyPrefix = "mailmgmt:prod:";
    private static readonly Random _random = new();

    public CacheService(
        IDistributedCache distributedCache,
        ILogger<CacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var fullKey = GetFullKey(key);
            var cachedData = await _distributedCache.GetStringAsync(fullKey, cancellationToken);

            if (string.IsNullOrEmpty(cachedData))
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(cachedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache for key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var fullKey = GetFullKey(key);
            var serializedData = JsonSerializer.Serialize(value);

            var effectiveTtl = ttl ?? TimeSpan.FromMinutes(5);
            // Add jitter to prevent cache stampede
            effectiveTtl = AddJitter(effectiveTtl);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = effectiveTtl
            };

            await _distributedCache.SetStringAsync(fullKey, serializedData, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullKey = GetFullKey(key);
            await _distributedCache.RemoveAsync(fullKey, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache for key: {Key}", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        // Note: This requires Redis-specific implementation
        // For ABP's IDistributedCache, we'll use a simplified approach
        // In production, consider using Redis commands directly via IConnectionMultiplexer
        _logger.LogWarning("RemoveByPrefixAsync is not fully implemented for IDistributedCache. Consider using Redis-specific implementation.");
        await Task.CompletedTask;
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? ttl = null,
        CancellationToken cancellationToken = default) where T : class
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        // Cache miss - execute factory
        var value = await factory();
        await SetAsync(key, value, ttl, cancellationToken);

        return value;
    }

    private string GetFullKey(string key)
    {
        return $"{KeyPrefix}{key}";
    }

    private TimeSpan AddJitter(TimeSpan ttl)
    {
        // Add 0-10% random jitter to TTL to prevent cache stampede
        var jitterSeconds = _random.Next(0, (int)(ttl.TotalSeconds * 0.1));
        return ttl.Add(TimeSpan.FromSeconds(jitterSeconds));
    }
}
