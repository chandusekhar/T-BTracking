using Volo.Abp.Auditing;
using Volo.Abp.Modularity;

namespace TMT.Auditing
{
    [DependsOn(
        typeof(AbpAuditingModule)
        )]
    public class TMTAuditingModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            SkipAutoServiceRegistration = true;
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
        }
    }
}