using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace YANEDGE.EmailManagement.Domain.Webhook;

/// <summary>
/// Webhook订阅实体
/// Webhook subscription entity for external system event notifications
/// </summary>
public class WebhookSubscription : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 订阅名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 目标URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 密钥 (用于签名验证)
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 订阅的事件类型列表
    /// </summary>
    public List<string> SubscribedEvents { get; set; } = new();

    /// <summary>
    /// 自定义HTTP头
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// 重试策略配置 (JSON)
    /// </summary>
    public string? RetryPolicy { get; set; }

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryCount { get; set; } = 5;

    /// <summary>
    /// 超时时间(秒)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    protected WebhookSubscription()
    {
    }

    public WebhookSubscription(
        Guid id,
        string name,
        string url,
        string secret,
        List<string> subscribedEvents,
        Guid? tenantId = null
    ) : base(id)
    {
        Name = name;
        Url = url;
        Secret = secret;
        SubscribedEvents = subscribedEvents;
        TenantId = tenantId;
        IsActive = true;
        MaxRetryCount = 5;
        TimeoutSeconds = 30;
    }

    public void UpdateSubscription(
        string name,
        string url,
        List<string> subscribedEvents,
        bool isActive
    )
    {
        Name = name;
        Url = url;
        SubscribedEvents = subscribedEvents;
        IsActive = isActive;
    }

    public void UpdateSecret(string secret)
    {
        Secret = secret;
    }

    public void SetRetryPolicy(int maxRetryCount, int timeoutSeconds)
    {
        MaxRetryCount = maxRetryCount;
        TimeoutSeconds = timeoutSeconds;
    }
}
