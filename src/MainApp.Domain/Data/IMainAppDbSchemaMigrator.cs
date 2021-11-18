using System.Threading.Tasks;

namespace MainApp.Data
{
    public interface IMainAppDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
