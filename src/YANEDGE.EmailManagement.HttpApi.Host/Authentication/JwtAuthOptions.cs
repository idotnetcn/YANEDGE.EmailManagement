namespace YANEDGE.EmailManagement.Authentication;

/// <summary>
/// JWT authentication options.
/// </summary>
public class JwtAuthOptions
{
    public const string SectionName = "Authentication:JwtBearer";

    public string Authority { get; set; } = string.Empty;

    public string Audience { get; set; } = "YANEDGE.EmailManagement";

    public string Issuer { get; set; } = "YANEDGE.EmailManagement";

    public string SigningKey { get; set; } = "YANEDGE.EmailManagement.Dev.SigningKey.2026";

    public bool RequireHttpsMetadata { get; set; }

    public bool ValidateIssuer { get; set; } = true;

    public bool ValidateAudience { get; set; } = true;

    public bool ValidateIssuerSigningKey { get; set; } = true;

    public bool ValidateLifetime { get; set; } = true;

    public int ClockSkewSeconds { get; set; } = 300;

    public int AccessTokenLifetimeMinutes { get; set; } = 60;

    public string[] DefaultScopes { get; set; } = [];
}
