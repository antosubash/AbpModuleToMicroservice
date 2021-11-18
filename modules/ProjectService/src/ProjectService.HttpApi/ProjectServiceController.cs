using ProjectService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace ProjectService
{
    public abstract class ProjectServiceController : AbpControllerBase
    {
        protected ProjectServiceController()
        {
            LocalizationResource = typeof(ProjectServiceResource);
        }
    }
}
