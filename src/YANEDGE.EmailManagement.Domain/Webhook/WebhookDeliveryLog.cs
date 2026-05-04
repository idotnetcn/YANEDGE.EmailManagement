using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace YANEDGE.EmailManagement.Domain.Webhook;

/// <summary>
/// Webhook投递日志实体
/// Webhook delivery log entity for tracking webhook delivery status
/// </summary>
public class WebhookDeliveryLog : CreationAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 关联的订阅ID
    /// </summary>
    public Guid SubscriptionId { get; set; }

    /// <summary>
    /// 事件ID
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// 事件类型
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// 投递载荷 (JSON)
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// 投递状态: 0-待处理, 1-成功, 2-失败
    /// </summary>
    public WebhookDeliveryStatus DeliveryStatus { get; set; }

    /// <summary>
    /// HTTP状态码
    /// </summary>
    public int? HttpStatusCode { get; set; }

    /// <summary>
    /// 响应体
    /// </summary>
    public string? ResponseBody { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// 下次重试时间
    /// </summary>
    public DateTime? NextRetryTime { get; set; }

    /// <summary>
    /// 投递完成时间
    /// </summary>
    public DateTime? DeliveredTime { get; set; }

    /// <summary>
    /// 目标URL (冗余字段，便于查询)
    /// </summary>
    public string Url { get; set; } = string.Empty;

    protected WebhookDeliveryLog()
    {
    }

    public WebhookDeliveryLog(
        Guid id,
        Guid subscriptionId,
        Guid eventId,
        string eventType,
        string payload,
        string url,
        Guid? tenantId = null
    ) : base(id)
    {
        SubscriptionId = subscriptionId;
        EventId = eventId;
        EventType = eventType;
        Payload = payload;
        Url = url;
        TenantId = tenantId;
        DeliveryStatus = WebhookDeliveryStatus.Pending;
        RetryCount = 0;
    }

    public void MarkAsSuccessful(int httpStatusCode, string? responseBody)
    {
        DeliveryStatus = WebhookDeliveryStatus.Successful;
        HttpStatusCode = httpStatusCode;
        ResponseBody = responseBody;
        DeliveredTime = DateTime.UtcNow;
        NextRetryTime = null;
    }

    public void MarkAsFailed(string errorMessage, int retryCount, DateTime? nextRetryTime)
    {
        DeliveryStatus = WebhookDeliveryStatus.Failed;
        ErrorMessage = errorMessage;
        RetryCount = retryCount;
        NextRetryTime = nextRetryTime;
    }

    public void MarkAsFailed(int httpStatusCode, string? responseBody, string errorMessage, int retryCount, DateTime? nextRetryTime)
    {
        DeliveryStatus = WebhookDeliveryStatus.Failed;
        HttpStatusCode = httpStatusCode;
        ResponseBody = responseBody;
        ErrorMessage = errorMessage;
        RetryCount = retryCount;
        NextRetryTime = nextRetryTime;
    }
}

/// <summary>
/// Webhook投递状态枚举
/// </summary>
public enum WebhookDeliveryStatus
{
    /// <summary>
    /// 待处理
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 投递成功
    /// </summary>
    Successful = 1,

    /// <summary>
    /// 投递失败
    /// </summary>
    Failed = 2
}
