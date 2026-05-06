using System.Security.Claims;
using Moq;
using Shouldly;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Authorization.Permissions;
using Xunit;
using YANEDGE.EmailManagement.Authentication;
using YANEDGE.EmailManagement.Permissions;

namespace YANEDGE.EmailManagement.Domain.Tests.Authentication;

public class JwtPermissionValueProviderTests : EmailManagementDomainTestBase
{
    private readonly JwtPermissionValueProvider _provider;

    public JwtPermissionValueProviderTests()
    {
        _provider = new JwtPermissionValueProvider(Mock.Of<IPermissionStore>());
    }

    [Fact]
    public async Task Should_Grant_When_Exact_Permission_Claim_Exists()
    {
        var principal = CreatePrincipal(EmailManagementPermissions.Templates.Create);
        var permission = CreatePermission(EmailManagementPermissions.Templates.Create);

        var result = await _provider.CheckAsync(new PermissionValueCheckContext(permission, principal));

        result.ShouldBe(PermissionGrantResult.Granted);
    }

    [Fact]
    public async Task Should_Grant_When_Parent_Permission_Claim_Exists()
    {
        var principal = CreatePrincipal(EmailManagementPermissions.Templates.Default);
        var permission = CreatePermission(EmailManagementPermissions.Templates.Create);

        var result = await _provider.CheckAsync(new PermissionValueCheckContext(permission, principal));

        result.ShouldBe(PermissionGrantResult.Granted);
    }

    [Fact]
    public async Task Should_Grant_For_Admin_Role()
    {
        var principal = CreatePrincipal(role: "admin");
        var permission = CreatePermission(EmailManagementPermissions.Templates.Delete);

        var result = await _provider.CheckAsync(new PermissionValueCheckContext(permission, principal));

        result.ShouldBe(PermissionGrantResult.Granted);
    }

    [Fact]
    public async Task Should_Return_Undefined_When_Permission_Claim_Does_Not_Exist()
    {
        var principal = CreatePrincipal(EmailManagementPermissions.Templates.Default);
        var permission = CreatePermission(EmailManagementPermissions.MailAccounts.Sync);

        var result = await _provider.CheckAsync(new PermissionValueCheckContext(permission, principal));

        result.ShouldBe(PermissionGrantResult.Undefined);
    }

    [Fact]
    public async Task Should_Grant_Multiple_Permissions_From_Claims()
    {
        var principal = CreatePrincipal(
            [EmailManagementPermissions.Templates.Default, EmailManagementPermissions.MailAccounts.Sync]);

        var permissions = new List<PermissionDefinition>
        {
            CreatePermission(EmailManagementPermissions.Templates.Create),
            CreatePermission(EmailManagementPermissions.MailAccounts.Sync),
            CreatePermission(EmailManagementPermissions.Statistics.View)
        };

        var result = await _provider.CheckAsync(new PermissionValuesCheckContext(permissions, principal));

        result.Result[EmailManagementPermissions.Templates.Create].ShouldBe(PermissionGrantResult.Granted);
        result.Result[EmailManagementPermissions.MailAccounts.Sync].ShouldBe(PermissionGrantResult.Granted);
        result.Result[EmailManagementPermissions.Statistics.View].ShouldBe(PermissionGrantResult.Undefined);
    }

    private static ClaimsPrincipal CreatePrincipal(string? permission = null, string? role = null)
    {
        return CreatePrincipal(
            permission is null ? [] : [permission],
            role is null ? [] : [role]);
    }

    private static ClaimsPrincipal CreatePrincipal(IEnumerable<string> permissions, IEnumerable<string>? roles = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, "tester")
        };

        claims.AddRange(permissions.Select(static permission => new Claim(EmailManagementClaimTypes.Permission, permission)));
        claims.AddRange((roles ?? []).Select(static r => new Claim(ClaimTypes.Role, r)));

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }

    private static PermissionDefinition CreatePermission(string name)
    {
        return (PermissionDefinition)Activator.CreateInstance(
            typeof(PermissionDefinition),
            bindingAttr: System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            binder: null,
            args: [name, null, MultiTenancySides.Both, true],
            culture: null)!;
    }
}
