using Volo.Abp.AspNetCore.Mvc;

namespace YANEDGE.EmailManagement.Controllers;

public abstract class EmailManagementController : AbpControllerBase
{
    protected EmailManagementController()
    {
        LocalizationResource = typeof(EmailManagementResource);
    }
}

