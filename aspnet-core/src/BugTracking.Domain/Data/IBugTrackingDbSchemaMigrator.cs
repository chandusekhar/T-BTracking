using System.Threading.Tasks;

namespace BugTracking.Data
{
    public interface IBugTrackingDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
