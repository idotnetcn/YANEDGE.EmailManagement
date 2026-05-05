using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Volo.Abp.AspNetCore.Mvc;
using YANEDGE.EmailManagement.Authentication;

namespace YANEDGE.EmailManagement.Controllers;

[Route("api/mail-management/v1/auth")]
public class AuthController : AbpControllerBase
{
    private readonly DevTokenService _devTokenService;

    public AuthController(DevTokenService devTokenService)
    {
        _devTokenService = devTokenService;
    }

    [AllowAnonymous]
    [HttpPost("dev-token")]
    public Task<DevTokenResultDto> CreateDevTokenAsync([FromBody] DevTokenRequestDto input)
    {
        return _devTokenService.IssueAsync(input);
    }

    [Authorize]
    [HttpGet("me")]
    public CurrentUserInfoDto GetCurrentUserAsync()
    {
        var user = CurrentUser;
        var principal = HttpContext.User;

        return new CurrentUserInfoDto
        {
            IsAuthenticated = user.IsAuthenticated,
            UserId = user.Id?.ToString() ?? principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub"),
            UserName = user.UserName ?? principal.Identity?.Name ?? principal.FindFirstValue(ClaimTypes.Name),
            AuthenticationType = principal.Identity?.AuthenticationType,
            Roles = principal.FindAll(ClaimTypes.Role).Select(static x => x.Value).Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            Permissions = principal.FindAll(EmailManagementClaimTypes.Permission).Select(static x => x.Value).Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            Scopes = principal.FindAll("scope").Select(static x => x.Value).Distinct(StringComparer.OrdinalIgnoreCase).ToList()
        };
    }
}
