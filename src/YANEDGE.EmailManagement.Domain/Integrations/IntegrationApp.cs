using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace YANEDGE.EmailManagement.Integrations;

public class IntegrationApp : FullAuditedAggregateRoot<Guid>
{
    public Guid? TenantId { get; set; }
    public string AppId { get; private set; } = null!;
    public string AppName { get; set; } = null!;
    public string EncryptedAppSecret { get; set; } = null!;
    public string? SourceSystem { get; set; }
    public string? CallbackUrl { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? AllowedIps { get; set; }
    public string? Description { get; set; }
    public string? ExtraProperties { get; set; }

    protected IntegrationApp() { }

    public IntegrationApp(Guid id, string appId, string appName, string encryptedAppSecret)
    {
        Id = id;
        AppId = appId;
        AppName = appName;
        EncryptedAppSecret = encryptedAppSecret;
    }
}
