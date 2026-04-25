using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using Volo.Abp.Validation;
using YANEDGE.EmailManagement.Localization;

namespace YANEDGE.EmailManagement;

[DependsOn(
    typeof(AbpLocalizationAbstractionsModule),
    typeof(AbpValidationModule)
)]
public class EmailManagementDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<EmailManagementResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource));
        });
    }
}
