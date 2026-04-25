using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace YANEDGE.EmailManagement;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpIdentityDomainModule),
    typeof(EmailManagementDomainSharedModule)
)]
public class EmailManagementDomainModule : AbpModule
{
}
