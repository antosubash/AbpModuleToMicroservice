using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.VirtualFileSystem;
using ProjectService;
using Volo.Abp.Http.Client;

namespace MainApp
{
    [DependsOn(
        typeof(AbpHttpClientModule),
        typeof(MainAppApplicationContractsModule),
        typeof(AbpAccountHttpApiClientModule),
        typeof(AbpIdentityHttpApiClientModule),
        typeof(AbpPermissionManagementHttpApiClientModule),
        typeof(AbpTenantManagementHttpApiClientModule),
        typeof(AbpFeatureManagementHttpApiClientModule),
        typeof(AbpSettingManagementHttpApiClientModule),
        typeof(ProjectServiceApplicationContractsModule)
    )]
    public class MainAppHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "Default";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(MainAppApplicationContractsModule).Assembly,
                RemoteServiceName
            );

            //Create dynamic client proxies
            context.Services.AddHttpClientProxies(
                typeof(ProjectServiceApplicationContractsModule).Assembly,
                "ProjectService"
            );

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<MainAppHttpApiClientModule>();
            });
        }
    }
}
