using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace YANEDGE.EmailManagement.Middleware;

/// <summary>
/// 速率限制中间件
/// 基于IP地址和用户ID实现API请求速率限制，防止恶意攻击和滥用
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger,
        RateLimitOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context, IDistributedCache cache)
    {
        // 跳过健康检查和Hangfire仪表盘端点
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/health") || path.StartsWith("/hangfire") || path.StartsWith("/swagger"))
        {
            await _next(context);
            return;
        }

        // 获取客户端标识符（IP地址或用户ID）
        var clientId = GetClientIdentifier(context);
        var endpoint = context.Request.Path.Value ?? "unknown";

        // 获取适用的速率限制规则
        var limit = GetRateLimitForEndpoint(endpoint);

        // 构建缓存键
        var cacheKey = $"rate_limit:{clientId}:{endpoint}";

        try
        {
            // 获取当前计数
            var currentCountStr = await cache.GetStringAsync(cacheKey);
            int currentCount = string.IsNullOrEmpty(currentCountStr) ? 0 : int.Parse(currentCountStr);

            if (currentCount >= limit.MaxRequests)
            {
                // 超过限制，返回429错误
                _logger.LogWarning(
                    "速率限制: 客户端 {ClientId} 超过了端点 {Endpoint} 的请求限制 ({MaxRequests}/{Window}秒)",
                    clientId, endpoint, limit.MaxRequests, limit.WindowSeconds);

                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    code = "RATE_LIMIT_EXCEEDED",
                    message = "请求过于频繁，请稍后再试",
                    retryAfter = limit.WindowSeconds,
                    timestamp = DateTime.UtcNow
                };

                var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(jsonResponse);
                return;
            }

            // 增加计数
            currentCount++;
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(limit.WindowSeconds)
            };
            await cache.SetStringAsync(cacheKey, currentCount.ToString(), cacheOptions);

            // 添加速率限制响应头
            context.Response.Headers["X-RateLimit-Limit"] = limit.MaxRequests.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = (limit.MaxRequests - currentCount).ToString();
            context.Response.Headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow.AddSeconds(limit.WindowSeconds).ToUnixTimeSeconds().ToString();

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "速率限制中间件处理失败");
            // 发生错误时继续处理请求，避免因缓存问题导致服务不可用
            await _next(context);
        }
    }

    /// <summary>
    /// 获取客户端标识符
    /// 优先使用用户ID，其次使用IP地址
    /// </summary>
    private string GetClientIdentifier(HttpContext context)
    {
        // 优先使用已认证用户的ID
        var userId = context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            return $"user:{userId}";
        }

        // 获取真实IP地址（考虑代理和负载均衡）
        var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Connection.RemoteIpAddress?.ToString();
        }

        return $"ip:{ipAddress ?? "unknown"}";
    }

    /// <summary>
    /// 根据端点获取速率限制规则
    /// </summary>
    private RateLimitRule GetRateLimitForEndpoint(string endpoint)
    {
        // 优先匹配自定义规则
        foreach (var rule in _options.EndpointRules)
        {
            if (endpoint.StartsWith(rule.Key, StringComparison.OrdinalIgnoreCase))
            {
                return rule.Value;
            }
        }

        // 返回默认规则
        return _options.DefaultRule;
    }
}

/// <summary>
/// 速率限制配置选项
/// </summary>
public class RateLimitOptions
{
    /// <summary>
    /// 默认速率限制规则
    /// </summary>
    public RateLimitRule DefaultRule { get; set; } = new RateLimitRule
    {
        MaxRequests = 100,
        WindowSeconds = 60
    };

    /// <summary>
    /// 端点特定的速率限制规则
    /// Key: 端点路径前缀，Value: 速率限制规则
    /// </summary>
    public Dictionary<string, RateLimitRule> EndpointRules { get; set; } = new Dictionary<string, RateLimitRule>
    {
        // API端点：每分钟100个请求
        ["/api"] = new RateLimitRule { MaxRequests = 100, WindowSeconds = 60 },

        // 认证端点：每5分钟10个请求（防止暴力破解）
        ["/api/account/login"] = new RateLimitRule { MaxRequests = 10, WindowSeconds = 300 },

        // 邮件发送端点：每分钟10个请求
        ["/api/mail-compose/send"] = new RateLimitRule { MaxRequests = 10, WindowSeconds = 60 },

        // 附件上传端点：每分钟20个请求
        ["/api/attachments/upload"] = new RateLimitRule { MaxRequests = 20, WindowSeconds = 60 },

        // 搜索端点：每分钟30个请求
        ["/api/mail-messages/search"] = new RateLimitRule { MaxRequests = 30, WindowSeconds = 60 }
    };
}

/// <summary>
/// 速率限制规则
/// </summary>
public class RateLimitRule
{
    /// <summary>
    /// 时间窗口内允许的最大请求数
    /// </summary>
    public int MaxRequests { get; set; }

    /// <summary>
    /// 时间窗口（秒）
    /// </summary>
    public int WindowSeconds { get; set; }
}

/// <summary>
/// 速率限制中间件扩展方法
/// </summary>
public static class RateLimitingMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder, RateLimitOptions? options = null)
    {
        options ??= new RateLimitOptions();
        return builder.UseMiddleware<RateLimitingMiddleware>(options);
    }
}
