using BugTracking.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace BugTracking.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(BugTrackingEntityFrameworkCoreDbMigrationsModule),
        typeof(AbpAuditLoggingEntityFrameworkCoreModule),
        typeof(BugTrackingApplicationContractsModule)

        )]
    public class BugTrackingDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
