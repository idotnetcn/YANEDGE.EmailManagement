using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Integrations;

public class IntegrationEventLog : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid IntegrationAppId { get; set; }
    public string EventName { get; set; } = null!;
    public string? EventPayload { get; set; }
    public byte DeliveryStatus { get; set; }
    public int RetryCount { get; set; }
    public string? LastError { get; set; }
    public DateTime CreationTime { get; set; }

    protected IntegrationEventLog() { }

    public IntegrationEventLog(Guid id, Guid integrationAppId, string eventName)
    {
        Id = id;
        IntegrationAppId = integrationAppId;
        EventName = eventName;
        CreationTime = DateTime.UtcNow;
    }
}
