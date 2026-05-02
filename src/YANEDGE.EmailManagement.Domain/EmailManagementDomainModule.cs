using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace YANEDGE.EmailManagement;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(EmailManagementDomainSharedModule)
)]
public class EmailManagementDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
    }
}
