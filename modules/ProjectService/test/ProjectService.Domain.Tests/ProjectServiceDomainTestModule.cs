using ProjectService.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace ProjectService
{
    /* Domain tests are configured to use the EF Core provider.
     * You can switch to MongoDB, however your domain tests should be
     * database independent anyway.
     */
    [DependsOn(
        typeof(ProjectServiceEntityFrameworkCoreTestModule)
        )]
    public class ProjectServiceDomainTestModule : AbpModule
    {
        
    }
}
