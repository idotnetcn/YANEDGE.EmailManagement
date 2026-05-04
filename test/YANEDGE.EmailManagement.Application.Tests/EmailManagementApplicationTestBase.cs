using Volo.Abp.Testing;

namespace YANEDGE.EmailManagement.Application.Tests;

public abstract class EmailManagementApplicationTestBase : AbpIntegratedTest<EmailManagementApplicationTestModule>
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }
}
