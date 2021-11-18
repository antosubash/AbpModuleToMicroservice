using MainApp.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MainApp.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class MainAppController : AbpControllerBase
    {
        protected MainAppController()
        {
            LocalizationResource = typeof(MainAppResource);
        }
    }
}