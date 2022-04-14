using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace BugTracking.EntityFrameworkCore
{
    [DependsOn(
        typeof(BugTrackingEntityFrameworkCoreModule)
        )]
    public class BugTrackingEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<BugTrackingMigrationsDbContext>();
        }
    }
}
