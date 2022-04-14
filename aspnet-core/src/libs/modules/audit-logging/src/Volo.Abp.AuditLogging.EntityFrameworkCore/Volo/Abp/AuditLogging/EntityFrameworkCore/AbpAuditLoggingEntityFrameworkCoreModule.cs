using Microsoft.Extensions.DependencyInjection;
using TMT.EntityFramworkCore;
using Volo.Abp.Modularity;

namespace Volo.Abp.AuditLogging.EntityFrameworkCore
{
    [DependsOn(typeof(AbpAuditLoggingDomainModule))]
    [DependsOn(typeof(TMTEntityFrameworkCoreModule))]
    public class AbpAuditLoggingEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<AbpAuditLoggingDbContext>(options =>
            {
                options.AddRepository<AuditLog, EfCoreAuditLogRepository>();
            });
        }
    }
}