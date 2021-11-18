using ProjectService.Localization;
using Volo.Abp.Application.Services;

namespace ProjectService
{
    public abstract class ProjectServiceAppService : ApplicationService
    {
        protected ProjectServiceAppService()
        {
            LocalizationResource = typeof(ProjectServiceResource);
            ObjectMapperContext = typeof(ProjectServiceApplicationModule);
        }
    }
}
