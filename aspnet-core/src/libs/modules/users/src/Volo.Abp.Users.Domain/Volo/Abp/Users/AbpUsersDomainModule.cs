using TMT.Ddd.Domain;
using Volo.Abp.Modularity;

namespace Volo.Abp.Users
{
    [DependsOn(
        typeof(AbpUsersDomainSharedModule),
        typeof(AbpUsersAbstractionModule),
        typeof(TMTDddDomainModule)
        )]
    public class AbpUsersDomainModule : AbpModule
    {
    }
}