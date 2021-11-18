using MainApp.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace MainApp.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(MainAppEntityFrameworkCoreModule),
        typeof(MainAppApplicationContractsModule)
        )]
    public class MainAppDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
