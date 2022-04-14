using Volo.Abp.Settings;

namespace BugTracking.Settings
{
    public class BugTrackingSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(BugTrackingSettings.MySetting1));
        }
    }
}
