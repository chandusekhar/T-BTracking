using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace TMT.Ddd.Domain
{
    [DependsOn(
            typeof(AbpDddDomainModule)
        )
        ]
    public class TMTDddDomainModule : AbpModule
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