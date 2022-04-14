using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace BugTracking
{
    [Dependency(ReplaceServices = true)]
    public class BugTrackingBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "BugTracking";
    }
}
