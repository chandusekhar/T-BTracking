using BugTracking.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace BugTracking
{
    [DependsOn(
        typeof(BugTrackingEntityFrameworkCoreTestModule)
        )]
    public class BugTrackingDomainTestModule : AbpModule
    {

    }
}