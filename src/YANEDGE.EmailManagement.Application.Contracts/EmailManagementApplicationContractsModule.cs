using Volo.Abp.Application;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace YANEDGE.EmailManagement;

[DependsOn(
    typeof(EmailManagementDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpIdentityApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule)
)]
public class EmailManagementApplicationContractsModule : AbpModule
{
}
