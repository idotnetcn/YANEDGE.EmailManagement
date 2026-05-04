using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using YANEDGE.EmailManagement.EntityFrameworkCore;

namespace YANEDGE.EmailManagement.Application.Tests;

[DependsOn(
    typeof(EmailManagementApplicationModule),
    typeof(EmailManagementEntityFrameworkCoreModule),
    typeof(AbpTestBaseModule)
)]
public class EmailManagementApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Use in-memory SQLite database for testing
        context.Services.AddEntityFrameworkInMemoryDatabase();

        var databaseName = Guid.NewGuid().ToString();

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(ctx =>
            {
                ctx.DbContextOptions.UseInMemoryDatabase(databaseName);
            });
        });
    }
}
