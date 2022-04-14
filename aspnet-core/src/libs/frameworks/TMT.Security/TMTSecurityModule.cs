using Microsoft.Extensions.DependencyInjection;
using TMT.Security.Claims;
using TMT.Security.Users;
using Volo.Abp.Modularity;

namespace TMT.Security
{
    public class TMTSecurityModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            SkipAutoServiceRegistration = true;
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<ITMTCurrentUser, TMTCurrentUser>();
            context.Services.AddSingleton<ITMTCurrentPrincipalAccessor, TMTThreadCurrentPrincipalAccessor>();
        }
    }
}