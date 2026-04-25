using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using YANEDGE.EmailManagement.EntityFrameworkCore;

namespace YANEDGE.EmailManagement.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(EmailManagementEntityFrameworkCoreModule),
    typeof(EmailManagementApplicationModule)
)]
public class DbMigratorModule : AbpModule
{
}
