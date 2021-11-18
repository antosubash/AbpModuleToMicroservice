using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MainApp.Data
{
    /* This is used if database provider does't define
     * IMainAppDbSchemaMigrator implementation.
     */
    public class NullMainAppDbSchemaMigrator : IMainAppDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}