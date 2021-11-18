using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace ProjectService
{
    [DependsOn(
        typeof(ProjectServiceApplicationContractsModule),
        typeof(AbpHttpClientModule))]
    public class ProjectServiceHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(ProjectServiceApplicationContractsModule).Assembly,
                ProjectServiceRemoteServiceConsts.RemoteServiceName
            );

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<ProjectServiceHttpApiClientModule>();
            });

        }
    }
}
