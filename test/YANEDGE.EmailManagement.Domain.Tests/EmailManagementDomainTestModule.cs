using Volo.Abp;
using Volo.Abp.Modularity;

namespace YANEDGE.EmailManagement.Domain.Tests;

[DependsOn(
    typeof(EmailManagementDomainModule),
    typeof(AbpTestBaseModule)
)]
public class EmailManagementDomainTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Configure test services here if needed
    }
}
