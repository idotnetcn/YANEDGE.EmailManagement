using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace YANEDGE.EmailManagement;

[DependsOn(
    typeof(EmailManagementApplicationContractsModule),
    typeof(EmailManagementDomainModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAutoMapperModule)
)]
public class EmailManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<EmailManagementApplicationModule>();
        });
    }
}
