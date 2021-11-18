using Volo.Abp.Modularity;

namespace ProjectService
{
    [DependsOn(
        typeof(ProjectServiceApplicationModule),
        typeof(ProjectServiceDomainTestModule)
        )]
    public class ProjectServiceApplicationTestModule : AbpModule
    {

    }
}
