using MainApp.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace MainApp.Web.Pages
{
    public abstract class MainAppPageModel : AbpPageModel
    {
        protected MainAppPageModel()
        {
            LocalizationResourceType = typeof(MainAppResource);
        }
    }
}