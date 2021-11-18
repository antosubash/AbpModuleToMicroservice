using ProjectService.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace ProjectService.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class ProjectServicePageModel : AbpPageModel
    {
        protected ProjectServicePageModel()
        {
            LocalizationResourceType = typeof(ProjectServiceResource);
            ObjectMapperContext = typeof(ProjectServiceWebModule);
        }
    }
}