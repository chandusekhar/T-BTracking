using BugTracking.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace BugTracking.Permissions
{
    public class BugTrackingPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(BugTrackingPermissions.GroupName);
            myGroup.AddPermission(BugTrackingPermissions.Users.Default);
            //Define your own permissions here. Example:
            //myGroup.AddPermission(BugTrackingPermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<BugTrackingResource>(name);
        }
    }
}
