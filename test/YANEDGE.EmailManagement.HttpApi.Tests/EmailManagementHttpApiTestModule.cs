using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.AspNetCore.TestBase;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using YANEDGE.EmailManagement.EntityFrameworkCore;

namespace YANEDGE.EmailManagement.HttpApi.Tests;

[DependsOn(
    typeof(EmailManagementHttpApiModule),
    typeof(EmailManagementApplicationModule),
    typeof(EmailManagementEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class EmailManagementHttpApiTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Use in-memory database for testing
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

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        app.UseRouting();
        app.UseAuthorization();
        app.UseConfiguredEndpoints();
    }
}
