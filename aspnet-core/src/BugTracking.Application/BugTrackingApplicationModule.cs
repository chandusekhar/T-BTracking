using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TMT.Auditing;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Emailing;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace BugTracking
{
    [DependsOn(
        typeof(BugTrackingDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(BugTrackingApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(AbpSettingManagementApplicationModule),
        typeof(TMTAuditingModule)
        )]
    public class BugTrackingApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<BugTrackingApplicationModule>();
            });
            //#if DEBUG
            //            context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
            //#endif
            context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, SmtpEmailSender>());
        }
    }
}
