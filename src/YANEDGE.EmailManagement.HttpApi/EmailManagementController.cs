using Volo.Abp.AspNetCore.Mvc;

namespace YANEDGE.EmailManagement;

public abstract class EmailManagementController : AbpControllerBase
{
    protected EmailManagementController()
    {
        LocalizationResource = typeof(EmailManagementDomainSharedModule);
    }
}
