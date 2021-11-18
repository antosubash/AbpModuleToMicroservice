using MainApp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace MainApp
{
    [DependsOn(
        typeof(MainAppEntityFrameworkCoreTestModule)
        )]
    public class MainAppDomainTestModule : AbpModule
    {

    }
}