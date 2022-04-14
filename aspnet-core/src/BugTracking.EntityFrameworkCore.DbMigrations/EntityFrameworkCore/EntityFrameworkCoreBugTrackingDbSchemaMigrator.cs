using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BugTracking.Data;
using Volo.Abp.DependencyInjection;

namespace BugTracking.EntityFrameworkCore
{
    public class EntityFrameworkCoreBugTrackingDbSchemaMigrator
        : IBugTrackingDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreBugTrackingDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the BugTrackingMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<BugTrackingMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}