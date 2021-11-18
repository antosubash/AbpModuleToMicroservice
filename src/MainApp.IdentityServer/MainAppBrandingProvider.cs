using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace MainApp
{
    [Dependency(ReplaceServices = true)]
    public class MainAppBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "MainApp";
    }
}
