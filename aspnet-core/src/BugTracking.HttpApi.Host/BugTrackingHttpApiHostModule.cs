using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BugTracking.EntityFrameworkCore;
using BugTracking.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.Auditing;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AutoMapper;
using BugTracking.SendMails;
using BugTracking.SendMail;
using System.Reflection;
using BugTracking.Constants;
using BugTracking.Options;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs.Hangfire;
using Microsoft.AspNetCore.Mvc.Versioning;
using Elsa;

namespace BugTracking
{
    [DependsOn(
        typeof(BugTrackingHttpApiModule),
        typeof(AbpAutofacModule),
        typeof(AbpAspNetCoreMultiTenancyModule),
        typeof(BugTrackingApplicationModule),
        typeof(BugTrackingEntityFrameworkCoreDbMigrationsModule),
        typeof(AbpAspNetCoreMvcUiBasicThemeModule),
        typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
        typeof(AbpAccountWebIdentityServerModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpSwashbuckleModule),
        typeof(AbpBackgroundJobsHangfireModule)
    )]
    public class BugTrackingHttpApiHostModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            ConfigureBundles();

            //ConfigureUrls(configuration);

            ConfigureConventionalControllers();

            ConfigureAuthentication(context, configuration);
            ConfigureLocalization();
            ConfigureVirtualFileSystem(context);
            ConfigureCors(context, configuration);
            ConfigureSwaggerServices(context, configuration);
            ConfigureExceptionHandlingServices();
            Configure<AbpAuditingOptions>(options =>
            {
                options.EntityHistorySelectors.AddAllEntities();
            });

            AddAuthorization(context);
            SettingIdentityClient(context, configuration);

            Configure<AbpAntiForgeryOptions>(options =>
            {
                options.AutoValidate = false;
                options.TokenCookie.Expiration = TimeSpan.FromDays(365);
                options.AutoValidateIgnoredHttpMethods.Remove("GET");
                options.AutoValidateFilter =
                    type => !type.Namespace.StartsWith("BugTracking.Controllers");
            });

            ConfigureHangfire(context, configuration);
            //ConfigureElsa(context, configuration);

        }
        //private void ConfigureElsa(ServiceConfigurationContext context, IConfiguration configuration)
        //{
        //    var elsaSection = configuration.GetSection("Elsa");

        //    context.Services.AddElsa(elsa =>
        //    {
        //        elsa
        //            .UseEntityFrameworkPersistence(ef =>
        //                DbContextOptionsBuilderExtensions.UseSqlServer(ef,
        //                    configuration.GetConnectionString("Default")))
        //            .AddConsoleActivities()
        //            .AddHttpActivities(elsaSection.GetSection("Server").Bind)
        //            .AddActivity<Activities.SendMail>()
        //            .AddQuartzTemporalActivities()
        //            .AddJavaScriptActivities()
        //            .AddWorkflowsFrom<Startup>();
        //    });

        //    context.Services.AddElsaApiEndpoints();
        //    //context.Services.AddElsaSwagger();
        //    context.Services.Configure<ApiVersioningOptions>(options =>
        //    {
        //        options.UseApiBehavior = false;
        //    });

        //    context.Services.AddCors(cors => cors.AddDefaultPolicy(policy => policy
        //        .AllowAnyHeader()
        //        .AllowAnyMethod()
        //        .AllowAnyOrigin()
        //        .WithExposedHeaders("Content-Disposition"))
        //    );

        //    //register controllers inside elsa
        //    context.Services.AddAssemblyOf<Elsa.Server.Api.Endpoints.WorkflowRegistry.Get>();

        //    //Disable antiforgery validation for elsa
        //    Configure<AbpAntiForgeryOptions>(options =>
        //    {
        //        options.AutoValidateFilter = type =>
        //            type.Assembly != typeof(Elsa.Server.Api.Endpoints.WorkflowRegistry.Get).Assembly;
        //    });
        //}

        //private BackgroundJobServer _backgroundJobServer;
        private void ConfigureHangfire(ServiceConfigurationContext context, IConfiguration configuration)
        {

            context.Services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(configuration.GetConnectionString("Default"));
            });
            context.Services.AddDbContext<BugTrackingDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Default"),
                          sqlServerOptionsAction: sqlOptions =>
                          {
                              sqlOptions.EnableRetryOnFailure();
                          });
            });
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<BugTrackingHttpApiModule>();
            });

            context.Services.AddScoped<ISendMail, SendNotifyMailAppService>();
            //_backgroundJobServer = new BackgroundJobServer();
        }
       

        private void ConfigureExceptionHandlingServices()
        {
            Configure<AbpExceptionHandlingOptions>(options =>
            {
                options.SendExceptionsDetailsToClients = true;
            });
        }

        private void ConfigureBundles()
        {
            Configure<AbpBundlingOptions>(options =>
            {
                options.StyleBundles.Configure(
                    BasicThemeBundles.Styles.Global,
                    bundle => { bundle.AddFiles("/global-styles.css"); }
                );
            });
        }

        private void ConfigureUrls(IConfiguration configuration)
        {
            Configure<AppUrlOptions>(options =>
            {
                options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
                options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"].Split(','));

                //options.Applications["Angular"].RootUrl = configuration["App:ClientUrl"];
                //options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] = "account/reset-password";
            });
        }

        private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<BugTrackingDomainSharedModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}BugTracking.Domain.Shared"));
                    options.FileSets.ReplaceEmbeddedByPhysical<BugTrackingDomainModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}BugTracking.Domain"));
                    options.FileSets.ReplaceEmbeddedByPhysical<BugTrackingApplicationContractsModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}BugTracking.Application.Contracts"));
                    options.FileSets.ReplaceEmbeddedByPhysical<BugTrackingApplicationModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}BugTracking.Application"));
                });
            }
        }

        private void ConfigureConventionalControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(BugTrackingApplicationModule).Assembly);
            });
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    options.Audience = "BugTrackingApp";
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/notify")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
        {
            //context.Services.AddAbpSwaggerGenWithOAuth(
            //    configuration["AuthServer:Authority"],
            //    new Dictionary<string, string>
            //    {
            //        {"BugTracking", "BugTracking API"}
            //    },
            //    options =>
            //    {
            //        options.SwaggerDoc("v1", new OpenApiInfo { Title = "BugTracking API", Version = "v1" });
            //        options.DocInclusionPredicate((docName, description) => true);
            //        options.CustomSchemaIds(type => type.FullName);
            //    });

            context.Services.AddSwaggerGen(options =>
            {

                var apiSecurity = new OpenApiSecurityRequirement{
                          {
                             new OpenApiSecurityScheme{
                                 Reference = new OpenApiReference{
                                 Id = "Bearer", //The name of the previously defined security scheme.
                                 Type = ReferenceType.SecurityScheme
                            }
                          },new List<string>()  }};

                options.CustomSchemaIds(type => type.FullName);
                options.DocInclusionPredicate((docName, description) => true);

                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(apiSecurity);

                options.SwaggerDoc("v1", new OpenApiInfo { Title = "T-BTracking API", Version = "v1" });
                string xmlFile = $"BugTracking.HttpApi.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

            });
        }

        private void ConfigureLocalization()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
                options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
                options.Languages.Add(new LanguageInfo("en", "en", "English"));
                options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (UK)"));
                options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
                options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
                options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
                options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
                options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
                options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
                options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch", "de"));
                options.Languages.Add(new LanguageInfo("es", "es", "Español", "es"));
            });
        }

        private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    builder
                        .WithOrigins(
                            configuration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbpRequestLocalization();

            if (!env.IsDevelopment())
            {
                app.UseErrorPage();
            }

            app.UseCorrelationId();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(DefaultCorsPolicyName);
            app.UseAuthentication();
            app.UseJwtTokenMiddleware();

            if (MultiTenancyConsts.IsEnabled)
            {
                app.UseMultiTenancy();
            }

            app.UseUnitOfWork();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseAbpSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BugTracking API");

                var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
                c.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
                c.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
                c.OAuthScopes("BugTracking");
            });
            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseConfiguredEndpoints();
            app.UseHangfireDashboard();
            //app.UseHttpActivities();
            //app.UseConfiguredEndpoints(endpoints =>
            //{
            //    endpoints.MapFallbackToPage("/_Host");
            //});

        }

        private void AddAuthorization(ServiceConfigurationContext context)
        {
            context.Services.AddAuthorization(options =>
            {
                options.AddPolicy(AppConstants.PolicyName.IdentityServer,
                        policy =>
                        {
                            policy.RequireClaim("client_id", "IdentityManagement_Common");
                        }
                    );
            });
        }

        private void SettingIdentityClient(ServiceConfigurationContext context, IConfiguration configuration)
        {
            // identity client option
            var identityClientOption = new IdentityClient();
            configuration.GetSection(nameof(IdentityClient)).Bind(identityClientOption);
            context.Services.AddSingleton(identityClientOption);
        }
    }
}
