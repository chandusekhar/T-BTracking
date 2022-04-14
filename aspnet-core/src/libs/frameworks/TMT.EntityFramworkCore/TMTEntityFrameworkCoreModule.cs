using Microsoft.Extensions.DependencyInjection;
using TMT.Auditing;
using Volo.Abp.Auditing;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace TMT.EntityFramworkCore
{
    [DependsOn(
           typeof(AbpEntityFrameworkCoreModule)
       )
       ]
    public class TMTEntityFrameworkCoreModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            SkipAutoServiceRegistration = true;
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<IAuditPropertySetter, TMTAuditPropertySetter>();
        }
    }
}