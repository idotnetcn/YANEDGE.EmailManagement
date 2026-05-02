using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace YANEDGE.EmailManagement;

[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(EmailManagementDomainSharedModule)
)]
public class EmailManagementApplicationContractsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
    }
}
