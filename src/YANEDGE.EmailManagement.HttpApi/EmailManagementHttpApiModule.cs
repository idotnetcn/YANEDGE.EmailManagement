using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace YANEDGE.EmailManagement;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(EmailManagementApplicationContractsModule)
)]
public class EmailManagementHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
    }
}
