using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp;
using Volo.Abp.Modularity;
using YANEDGE.EmailManagement.Authentication;

namespace YANEDGE.EmailManagement.Domain.Tests;

[DependsOn(
    typeof(EmailManagementDomainModule),
    typeof(AbpTestBaseModule)
)]
public class EmailManagementDomainTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<JwtPermissionValueProvider>();

        Configure<AbpPermissionOptions>(options =>
        {
            options.ValueProviders.Add<JwtPermissionValueProvider>();
        });
    }
}
