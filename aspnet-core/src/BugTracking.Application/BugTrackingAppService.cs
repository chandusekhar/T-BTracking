using BugTracking.Localization;
using Volo.Abp.Application.Services;

namespace BugTracking
{
    /* Inherit your application services from this class.
     */
    public abstract class BugTrackingAppService : ApplicationService
    {
        protected BugTrackingAppService()
        {
            LocalizationResource = typeof(BugTrackingResource);
        }
    }
}
