using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Validation;

namespace YANEDGE.EmailManagement.Middleware;

/// <summary>
/// 全局异常处理中间件
/// 捕获所有未处理的异常并返回标准化的错误响应
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        var path = context.Request.Path;
        var method = context.Request.Method;
        var userId = context.User?.Identity?.Name ?? "Anonymous";

        // 记录异常日志
        _logger.LogError(exception,
            "未处理的异常: {ExceptionType} | TraceId: {TraceId} | UserId: {UserId} | Path: {Path} | Method: {Method}",
            exception.GetType().Name, traceId, userId, path, method);

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new StandardErrorResponse
        {
            TraceId = traceId,
            Timestamp = DateTime.UtcNow
        };

        // 根据异常类型设置响应
        switch (exception)
        {
            case BusinessException businessEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Code = businessEx.Code ?? "BUSINESS_ERROR";
                errorResponse.Message = businessEx.Message;
                errorResponse.Details = businessEx.Details;
                break;

            case AbpValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Code = "VALIDATION_ERROR";
                errorResponse.Message = "输入验证失败";
                errorResponse.Details = validationEx.Message;
                errorResponse.ValidationErrors = validationEx.ValidationErrors;
                break;

            case AbpAuthorizationException authEx:
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                errorResponse.Code = "AUTHORIZATION_ERROR";
                errorResponse.Message = "无权限访问";
                errorResponse.Details = _environment.IsDevelopment() ? authEx.Message : null;
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Code = "UNAUTHORIZED";
                errorResponse.Message = "未授权访问";
                break;

            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Code = "NOT_FOUND";
                errorResponse.Message = "请求的资源不存在";
                errorResponse.Details = _environment.IsDevelopment() ? exception.Message : null;
                break;

            case ArgumentException argumentEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Code = "INVALID_ARGUMENT";
                errorResponse.Message = "参数无效";
                errorResponse.Details = _environment.IsDevelopment() ? argumentEx.Message : null;
                break;

            case InvalidOperationException invalidOpEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Code = "INVALID_OPERATION";
                errorResponse.Message = "操作无效";
                errorResponse.Details = _environment.IsDevelopment() ? invalidOpEx.Message : null;
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Code = "INTERNAL_ERROR";
                errorResponse.Message = "服务器内部错误";
                // 生产环境不暴露详细错误信息
                errorResponse.Details = _environment.IsDevelopment() ? exception.ToString() : "请联系系统管理员";
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        });

        await response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// 标准错误响应模型
/// </summary>
public class StandardErrorResponse
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 错误消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 详细信息（开发环境可见）
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// 请求追踪ID
    /// </summary>
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 验证错误列表
    /// </summary>
    public object? ValidationErrors { get; set; }
}

/// <summary>
/// 中间件扩展方法
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
