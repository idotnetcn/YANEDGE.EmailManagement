using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp.DependencyInjection;
using YANEDGE.EmailManagement.Permissions;

namespace YANEDGE.EmailManagement.Authentication;

/// <summary>
/// Development-only token issuing endpoint.
/// </summary>
public class DevTokenService : ITransientDependency
{
    private readonly JwtAuthOptions _jwtAuthOptions;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public DevTokenService(IOptions<JwtAuthOptions> jwtAuthOptions, IWebHostEnvironment webHostEnvironment)
    {
        _jwtAuthOptions = jwtAuthOptions.Value;
        _webHostEnvironment = webHostEnvironment;
    }

    public Task<DevTokenResultDto> IssueAsync(DevTokenRequestDto input)
    {
        if (!_webHostEnvironment.IsDevelopment())
        {
            throw new InvalidOperationException("Dev token issuing is only available in development environment.");
        }

        var issuedAt = DateTime.UtcNow;
        var expiresAt = issuedAt.AddMinutes(input.ExpirationMinutes > 0
            ? input.ExpirationMinutes
            : _jwtAuthOptions.AccessTokenLifetimeMinutes);

        var permissions = input.Permissions?.Where(static p => !string.IsNullOrWhiteSpace(p)).Distinct(StringComparer.OrdinalIgnoreCase).ToList()
            ?? [];

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, input.UserId?.ToString() ?? Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.UniqueName, input.UserName),
            new(ClaimTypes.Name, input.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(issuedAt).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        foreach (var role in input.Roles?.Where(static r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase) ?? [])
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var permission in permissions)
        {
            claims.Add(new Claim(EmailManagementClaimTypes.Permission, permission));
        }

        foreach (var scope in input.Scopes?.Where(static s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase)
                     ?? _jwtAuthOptions.DefaultScopes)
        {
            claims.Add(new Claim("scope", scope));
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.SigningKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtAuthOptions.Issuer,
            audience: _jwtAuthOptions.Audience,
            claims: claims,
            notBefore: issuedAt,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return Task.FromResult(new DevTokenResultDto
        {
            AccessToken = tokenValue,
            TokenType = "Bearer",
            ExpiresAt = expiresAt,
            Audience = _jwtAuthOptions.Audience,
            Issuer = _jwtAuthOptions.Issuer,
            GrantedPermissions = permissions
        });
    }
}

public class DevTokenRequestDto
{
    public Guid? UserId { get; set; }

    public string UserName { get; set; } = "developer";

    public List<string> Roles { get; set; } = [];

    public List<string> Permissions { get; set; } =
    [
        EmailManagementPermissions.MailAccounts.Default,
        EmailManagementPermissions.MailAccounts.Manage,
        EmailManagementPermissions.MailAccounts.Sync,
        EmailManagementPermissions.Messages.Default,
        EmailManagementPermissions.Messages.View,
        EmailManagementPermissions.Threads.Default,
        EmailManagementPermissions.Threads.View,
        EmailManagementPermissions.Threads.Assign,
        EmailManagementPermissions.Threads.Claim,
        EmailManagementPermissions.Threads.Archive,
        EmailManagementPermissions.SendTasks.Default,
        EmailManagementPermissions.SendTasks.Create,
        EmailManagementPermissions.SendTasks.Send,
        EmailManagementPermissions.SendTasks.Approve,
        EmailManagementPermissions.Attachments.Default,
        EmailManagementPermissions.Attachments.Create,
        EmailManagementPermissions.Attachments.Update,
        EmailManagementPermissions.Attachments.Delete,
        EmailManagementPermissions.Attachments.Download,
        EmailManagementPermissions.Rules.Default,
        EmailManagementPermissions.Rules.Create,
        EmailManagementPermissions.Rules.Update,
        EmailManagementPermissions.Rules.Delete,
        EmailManagementPermissions.Rules.Manage,
        EmailManagementPermissions.Templates.Default,
        EmailManagementPermissions.Templates.Create,
        EmailManagementPermissions.Templates.Update,
        EmailManagementPermissions.Templates.Delete,
        EmailManagementPermissions.Templates.Manage,
        EmailManagementPermissions.Labels.Default,
        EmailManagementPermissions.Labels.Create,
        EmailManagementPermissions.Labels.Update,
        EmailManagementPermissions.Labels.Delete,
        EmailManagementPermissions.Contacts.Default,
        EmailManagementPermissions.Contacts.Create,
        EmailManagementPermissions.Contacts.Update,
        EmailManagementPermissions.Contacts.Delete,
        EmailManagementPermissions.BusinessRelations.Default,
        EmailManagementPermissions.BusinessRelations.Create,
        EmailManagementPermissions.BusinessRelations.Update,
        EmailManagementPermissions.BusinessRelations.Delete,
        EmailManagementPermissions.Integration.Default,
        EmailManagementPermissions.Integration.Manage,
        EmailManagementPermissions.Statistics.Default,
        EmailManagementPermissions.Statistics.View
    ];

    public List<string> Scopes { get; set; } = [];

    public int ExpirationMinutes { get; set; } = 60;
}

public class DevTokenResultDto
{
    public string AccessToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = "Bearer";

    public DateTime ExpiresAt { get; set; }

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public List<string> GrantedPermissions { get; set; } = [];
}
