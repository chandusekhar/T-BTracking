using Volo.Abp.Modularity;

namespace BugTracking
{
    [DependsOn(
        typeof(BugTrackingApplicationModule),
        typeof(BugTrackingDomainTestModule)
        )]
    public class BugTrackingApplicationTestModule : AbpModule
    {

    }
}