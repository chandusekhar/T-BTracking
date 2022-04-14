using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace BugTracking.Data
{
    /* This is used if database provider does't define
     * IBugTrackingDbSchemaMigrator implementation.
     */
    public class NullBugTrackingDbSchemaMigrator : IBugTrackingDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}