using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Integrations;

public class IntegrationWebhook : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid IntegrationAppId { get; private set; }
    public string EventName { get; set; } = null!;
    public string TargetUrl { get; set; } = null!;
    public string? SecretKey { get; set; }
    public bool RetryEnabled { get; set; } = true;
    public bool IsEnabled { get; set; } = true;
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }

    protected IntegrationWebhook() { }

    public IntegrationWebhook(Guid id, Guid integrationAppId, string eventName, string targetUrl)
    {
        Id = id;
        IntegrationAppId = integrationAppId;
        EventName = eventName;
        TargetUrl = targetUrl;
        CreationTime = DateTime.UtcNow;
    }
}
