using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace YANEDGE.EmailManagement;

[DependsOn(
    typeof(AbpDddDomainSharedModule)
)]
public class EmailManagementDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
    }
}
