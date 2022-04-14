using BugTracking.EntityFrameworkCore;
using BugTracking.Hub;
using BugTracking.SendMails;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BugTracking
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<BugTrackingHttpApiHostModule>();
            services.AddDbContext<BugTrackingDbContext>(ServiceLifetime.Transient);
            services.AddSignalR();
            services.AddApiVersioning(
               options =>
               {
                   options.ReportApiVersions = true;
                   options.AssumeDefaultVersionWhenUnspecified = true;
                   options.UseApiBehavior = false;
               });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
            IRecurringJobManager recurringJobManager)
        {
            app.UseHangfireServer();
            app.InitializeApplication();
            app.UseCors("CorsPolicy");
            app.UseCookiePolicy();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SignalR>("/notify");
                //endpoints.EnableDependencyInjection();
                //endpoints.Select().Expand().Filter().OrderBy().MaxTop(100).Count();
                //endpoints.MapODataRoute("odata", "odata", GetEdmModel());
            });
            recurringJobManager.AddOrUpdate<ISendMail>("AutoSendMail",
                x => x.AutoSendMailAsync(), Cron.Daily(11, 0));
            //app.UseHttpActivities();
        }
    }
}
