using Localization.Resources.AbpUi;
using ProjectService.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectService
{
    [DependsOn(
        typeof(ProjectServiceApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule))]
    public class ProjectServiceHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(ProjectServiceHttpApiModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<ProjectServiceResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });
        }
    }
}
