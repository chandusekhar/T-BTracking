using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using Volo.Abp.Users.EntityFrameworkCore;
using BugTracking.Comments;
using BugTracking.Assignees;
using BugTracking.DetailAttachments;
using BugTracking.Issues;
using BugTracking.Members;
using BugTracking.Projects;
using BugTracking.Notifications;
using BugTracking.Attachments;
using BugTracking.Follows;
using BugTracking.Users;
using BugTracking.Views;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using BugTracking.Azures;
using BugTracking.Departments;
using BugTracking.Teams;
using BugTracking.MemberTeams;
using BugTracking.TimeOnProjects;
using BugTracking.HistoryViews;
using BugTracking.HistoryDashboards;

namespace BugTracking.EntityFrameworkCore
{
    /* This is your actual DbContext used on runtime.
     * It includes only your entities.
     * It does not include entities of the used modules, because each module has already
     * its own DbContext class. If you want to share some database tables with the used modules,
     * just create a structure like done for AppUser.
     *
     * Don't use this DbContext for database migrations since it does not contain tables of the
     * used modules (as explained above). See BugTrackingMigrationsDbContext for migrations.
     */
    [ConnectionStringName("Default")]
    public class BugTrackingDbContext : AbpDbContext<BugTrackingDbContext>
    {
        public DbSet<IssueChangedView> IssueChangedViews { get; set; }
        public DbSet<HistoryDashboard> HistoryDashboards { get; set; }
        public DbSet <HistoryView> HistoryViews { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Assignee> Assignees { get; set; }
        public DbSet<Categories.Category> Categories { get; set; }
        public DbSet<DetailAttachment> DetailAttachments { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<MemberTeam> MemberTeams { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Statuss.Status> Status { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Azure> Azures { get; set; }
        public DbSet<Conversation.Conversation> conversations { get; set; }
        public DbSet<UserInforTFS.UserInforTfs> UserInforTfs { get; set; }
        public DbSet<TimeOnProject> timeOnProjects { get; set; }

        /* Add DbSet properties for your Aggregate Roots / Entities here.
         * Also map them inside BugTrackingDbContextModelCreatingExtensions.ConfigureBugTracking
         */

        public BugTrackingDbContext(DbContextOptions<BugTrackingDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.ApplyConfiguration(new IssueChangedViewMap());
            builder.Entity<HistoryView>().ToView(nameof(HistoryView)).HasNoKey();
            builder.Entity<HistoryDashboard>().ToView(nameof(HistoryDashboard)).HasNoKey();
            base.OnModelCreating(builder);
            builder
                .Entity<IssueChangedView>()
                .ToView(nameof(IssueChangedView))
                .HasNoKey();
            builder.ConfigureAuditLogging();
            /* Configure the shared tables (with included modules) here */

            builder.Entity<AppUser>(b =>
            {
                b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "Users"); //Sharing the same table "AbpUsers" with the IdentityUser

                b.ConfigureByConvention();
                b.ConfigureAbpUser();

                /* Configure mappings for your additional properties
                 * Also see the BugTrackingEfCoreEntityExtensionMappings class
                 */
            });

            /* Configure your own tables/entities inside the ConfigureBugTracking method */

            builder.ConfigureBugTracking();
        }
    }
}
