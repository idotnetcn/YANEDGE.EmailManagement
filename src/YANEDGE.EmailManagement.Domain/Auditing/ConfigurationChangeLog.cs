using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Auditing;

public class ConfigurationChangeLog : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public string ConfigType { get; set; } = null!;
    public string? ConfigKey { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? ChangeReason { get; set; }
    public DateTime CreationTime { get; set; }

    protected ConfigurationChangeLog() { }

    public ConfigurationChangeLog(Guid id, string configType)
    {
        Id = id;
        ConfigType = configType;
        CreationTime = DateTime.UtcNow;
    }
}
