using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BugTracking.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class BugTrackingMigrationsDbContextFactory : IDesignTimeDbContextFactory<BugTrackingMigrationsDbContext>
    {
        public BugTrackingMigrationsDbContext CreateDbContext(string[] args)
        {
            BugTrackingEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<BugTrackingMigrationsDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"));

            return new BugTrackingMigrationsDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BugTracking.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
