using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace YANEDGE.EmailManagement.Middleware;

/// <summary>
/// 安全头中间件
/// 添加常见安全响应头，防止XSS、点击劫持等攻击
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;
    private readonly SecurityHeadersOptions _options;

    public SecurityHeadersMiddleware(
        RequestDelegate next,
        ILogger<SecurityHeadersMiddleware> logger,
        SecurityHeadersOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 添加安全响应头
        var headers = context.Response.Headers;

        // X-Content-Type-Options: 防止MIME类型嗅探
        if (_options.EnableXContentTypeOptions)
        {
            headers["X-Content-Type-Options"] = "nosniff";
        }

        // X-Frame-Options: 防止点击劫持攻击
        if (_options.EnableXFrameOptions)
        {
            headers["X-Frame-Options"] = _options.XFrameOptionsValue;
        }

        // X-XSS-Protection: 启用浏览器XSS过滤器
        if (_options.EnableXssProtection)
        {
            headers["X-XSS-Protection"] = "1; mode=block";
        }

        // Strict-Transport-Security: 强制使用HTTPS
        if (_options.EnableHsts && context.Request.IsHttps)
        {
            headers["Strict-Transport-Security"] = $"max-age={_options.HstsMaxAge}; includeSubDomains; preload";
        }

        // Content-Security-Policy: 内容安全策略
        if (_options.EnableContentSecurityPolicy && !string.IsNullOrEmpty(_options.ContentSecurityPolicyValue))
        {
            headers["Content-Security-Policy"] = _options.ContentSecurityPolicyValue;
        }

        // Referrer-Policy: 控制Referer信息的发送
        if (_options.EnableReferrerPolicy)
        {
            headers["Referrer-Policy"] = _options.ReferrerPolicyValue;
        }

        // Permissions-Policy: 控制浏览器功能和API
        if (_options.EnablePermissionsPolicy && !string.IsNullOrEmpty(_options.PermissionsPolicyValue))
        {
            headers["Permissions-Policy"] = _options.PermissionsPolicyValue;
        }

        // X-Permitted-Cross-Domain-Policies: 控制跨域策略文件
        if (_options.EnableXPermittedCrossDomainPolicies)
        {
            headers["X-Permitted-Cross-Domain-Policies"] = "none";
        }

        // 移除可能泄露服务器信息的响应头
        if (_options.RemoveServerHeader)
        {
            headers.Remove("Server");
            headers.Remove("X-Powered-By");
            headers.Remove("X-AspNet-Version");
            headers.Remove("X-AspNetMvc-Version");
        }

        _logger.LogDebug("安全头已添加到响应中");

        await _next(context);
    }
}

/// <summary>
/// 安全头配置选项
/// </summary>
public class SecurityHeadersOptions
{
    /// <summary>
    /// 启用 X-Content-Type-Options
    /// </summary>
    public bool EnableXContentTypeOptions { get; set; } = true;

    /// <summary>
    /// 启用 X-Frame-Options
    /// </summary>
    public bool EnableXFrameOptions { get; set; } = true;

    /// <summary>
    /// X-Frame-Options 值 (DENY, SAMEORIGIN, ALLOW-FROM uri)
    /// </summary>
    public string XFrameOptionsValue { get; set; } = "SAMEORIGIN";

    /// <summary>
    /// 启用 X-XSS-Protection
    /// </summary>
    public bool EnableXssProtection { get; set; } = true;

    /// <summary>
    /// 启用 Strict-Transport-Security (HSTS)
    /// </summary>
    public bool EnableHsts { get; set; } = true;

    /// <summary>
    /// HSTS最大年龄（秒）
    /// 默认1年
    /// </summary>
    public int HstsMaxAge { get; set; } = 31536000;

    /// <summary>
    /// 启用 Content-Security-Policy
    /// </summary>
    public bool EnableContentSecurityPolicy { get; set; } = true;

    /// <summary>
    /// Content-Security-Policy 策略值
    /// </summary>
    public string ContentSecurityPolicyValue { get; set; } =
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
        "style-src 'self' 'unsafe-inline'; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' data:; " +
        "connect-src 'self'; " +
        "frame-ancestors 'self';";

    /// <summary>
    /// 启用 Referrer-Policy
    /// </summary>
    public bool EnableReferrerPolicy { get; set; } = true;

    /// <summary>
    /// Referrer-Policy 值
    /// </summary>
    public string ReferrerPolicyValue { get; set; } = "strict-origin-when-cross-origin";

    /// <summary>
    /// 启用 Permissions-Policy
    /// </summary>
    public bool EnablePermissionsPolicy { get; set; } = true;

    /// <summary>
    /// Permissions-Policy 值
    /// </summary>
    public string PermissionsPolicyValue { get; set; } =
        "geolocation=(), " +
        "microphone=(), " +
        "camera=(), " +
        "payment=(), " +
        "usb=(), " +
        "magnetometer=(), " +
        "gyroscope=(), " +
        "accelerometer=()";

    /// <summary>
    /// 启用 X-Permitted-Cross-Domain-Policies
    /// </summary>
    public bool EnableXPermittedCrossDomainPolicies { get; set; } = true;

    /// <summary>
    /// 移除可能泄露服务器信息的响应头
    /// </summary>
    public bool RemoveServerHeader { get; set; } = true;
}

/// <summary>
/// 安全头中间件扩展方法
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder, SecurityHeadersOptions? options = null)
    {
        options ??= new SecurityHeadersOptions();
        return builder.UseMiddleware<SecurityHeadersMiddleware>(options);
    }
}
