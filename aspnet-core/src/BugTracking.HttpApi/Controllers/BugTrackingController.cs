using BugTracking.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace BugTracking.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class BugTrackingController : AbpController
    {
        protected BugTrackingController()
        {
            LocalizationResource = typeof(BugTrackingResource);
        }
    }
}