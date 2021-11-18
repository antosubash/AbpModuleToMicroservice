using Volo.Abp.Modularity;

namespace MainApp
{
    [DependsOn(
        typeof(MainAppApplicationModule),
        typeof(MainAppDomainTestModule)
        )]
    public class MainAppApplicationTestModule : AbpModule
    {

    }
}