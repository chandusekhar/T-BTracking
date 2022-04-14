using BugTracking.Localization;
using BugTracking.AdminOData;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.TenantManagement;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Volo.Abp.Account;
using BugTracking.Users;
using BugTracking.Controllers;
using BugTracking.Projects;
using BugTracking.Issues;
using BugTracking.Departments;
using BugTracking.Teams;
using BugTracking.DepartmentOData;
using BugTracking.MemberTeams;

namespace BugTracking
{
    [DependsOn(
        typeof(BugTrackingApplicationContractsModule),
        typeof(AbpAccountHttpApiModule),
        typeof(AbpIdentityHttpApiModule),
        typeof(AbpPermissionManagementHttpApiModule),
        typeof(AbpTenantManagementHttpApiModule),
        typeof(AbpFeatureManagementHttpApiModule)
        )]
    public class BugTrackingHttpApiModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
          
            ConfigureLocalization();
            ConfigureControllerWithViews(context);
        }
        private void ConfigureLocalization()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<BugTrackingResource>()
                    .AddBaseTypes(
                        typeof(AbpUiResource)
                    );
            });
        }
        private void ConfigureControllerWithViews(ServiceConfigurationContext context)
        {
            context.Services.AddControllers().AddOData(opt =>
            {
                opt.Count()
                    .Filter()
                    .Expand()
                    .Select()
                    .OrderBy()
                    .SetMaxTop(1000)
                    .AddRouteComponents("odata", GetEdmModel())
                    .Conventions.Add(new CustomODataConvention());
                //opt.EnableAttributeRouting = true;
                opt.RouteOptions.EnableKeyAsSegment = false;
                opt.RouteOptions.EnableKeyInParenthesis = true;
                opt.RouteOptions.EnableQualifiedOperationCall = true;
                opt.RouteOptions.EnableUnqualifiedOperationCall = false;
            });
        }
        private IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder(); //mặc định
            builder.EntitySet<AdminODataDto>("AdminOData");
            var x = builder.EntityType<AdminODataDto>()
              .Collection
              .Function("getListProject")
              .ReturnsCollection<ProjectDto>();
               x.Namespace = "ODataService";
            var t = builder.EntityType<AdminODataDto>()
             .Collection
             .Function("GetListIssue")
             .Returns<IssueAdminODataDto>();
            t.Namespace = "ODataService";
            t.Parameter<string>("UserId");

            builder.EntitySet<DepartmentODataDto>("DepartmentOData");
            var er = builder.EntityType<DepartmentODataDto>()
             .Collection
             .Function("GetListTeam")
             .ReturnsCollection<TeamDto>();
            er.Namespace = "ODataService";
            var m = builder.EntityType<DepartmentODataDto>()
            .Collection
            .Function("GetListMemberByTeam")
            .ReturnsCollection<MemberTeamDto>();
            m.Namespace = "ODataService";

            builder.EntitySet<UserDto>("UserOData");

            //  builder.EntitySet<ProjectDto>("AdminOData");
            return builder.GetEdmModel();//mặc định
        }

        public class CustomODataConvention : IODataControllerActionConvention
        {
            /// <summary>
            /// Order value.
            /// </summary>
            public int Order => 999;

            /// <summary>
            /// Apply to action,.
            /// </summary>
            /// <param name="context">Http context.</param>
            /// <returns>true/false</returns>
            public bool AppliesToAction(ODataControllerActionContext context)
            {
                return true; // apply to all controller
            }

            /// <summary>
            /// Apply to controller
            /// </summary>
            /// <param name="context">Http context.</param>
            /// <returns>true/false</returns>
            public bool AppliesToController(ODataControllerActionContext context)
            {
                return true; // continue for all others
            }
        }
    }
}

