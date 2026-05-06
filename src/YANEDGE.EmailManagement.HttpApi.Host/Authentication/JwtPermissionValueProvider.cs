using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace YANEDGE.EmailManagement.Authentication;

/// <summary>
/// ABP permission value provider backed by JWT permission claims.
/// </summary>
public class JwtPermissionValueProvider : PermissionValueProvider, ITransientDependency
{
    public const string ProviderName = "JwtClaim";

    public JwtPermissionValueProvider(IPermissionStore permissionStore)
        : base(permissionStore)
    {
    }

    public override string Name => ProviderName;

    public override Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        if (HasPermission(context.Principal, context.Permission.Name))
        {
            return Task.FromResult(PermissionGrantResult.Granted);
        }

        return Task.FromResult(PermissionGrantResult.Undefined);
    }

    public override Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        var result = new MultiplePermissionGrantResult();

        foreach (var permission in context.Permissions)
        {
            result.Result[permission.Name] = HasPermission(context.Principal, permission.Name)
                ? PermissionGrantResult.Granted
                : PermissionGrantResult.Undefined;
        }

        return Task.FromResult(result);
    }

    private static bool HasPermission(System.Security.Claims.ClaimsPrincipal principal, string permissionName)
    {
        if (principal.IsInRole("admin") || principal.IsInRole("Admin"))
        {
            return true;
        }

        var grantedPermissions = principal.FindAll(EmailManagementClaimTypes.Permission)
            .Select(static x => x.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (grantedPermissions.Contains(permissionName))
        {
            return true;
        }

        var current = permissionName;
        while (current.Contains('.'))
        {
            current = current[..current.LastIndexOf('.')];
            if (grantedPermissions.Contains(current))
            {
                return true;
            }
        }

        return false;
    }
}
