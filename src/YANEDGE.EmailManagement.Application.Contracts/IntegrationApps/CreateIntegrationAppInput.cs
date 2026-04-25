using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.IntegrationApps;

public class CreateIntegrationAppInput
{
    [Required, MaxLength(64)]
    public string AppId { get; set; } = null!;

    [Required, MaxLength(128)]
    public string AppName { get; set; } = null!;

    [MaxLength(64)]
    public string? SourceSystem { get; set; }

    [MaxLength(512)]
    public string? CallbackUrl { get; set; }

    [MaxLength(512)]
    public string? Description { get; set; }
}
