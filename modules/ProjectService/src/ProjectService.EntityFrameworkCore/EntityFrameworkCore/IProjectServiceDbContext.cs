using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace ProjectService.EntityFrameworkCore
{
    [ConnectionStringName(ProjectServiceDbProperties.ConnectionStringName)]
    public interface IProjectServiceDbContext : IEfCoreDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * DbSet<Question> Questions { get; }
         */
    }
}