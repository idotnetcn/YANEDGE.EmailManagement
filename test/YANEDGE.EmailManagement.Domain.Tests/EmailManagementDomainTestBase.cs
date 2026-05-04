using Volo.Abp.Testing;

namespace YANEDGE.EmailManagement.Domain.Tests;

public abstract class EmailManagementDomainTestBase : AbpIntegratedTest<EmailManagementDomainTestModule>
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }
}
