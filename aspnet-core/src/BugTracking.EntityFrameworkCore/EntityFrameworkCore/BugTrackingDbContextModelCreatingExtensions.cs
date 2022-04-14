using BugTracking.Assignees;
using BugTracking.Attachments;
using BugTracking.Categories;
using BugTracking.Comments;
using BugTracking.DetailAttachments;
using BugTracking.Issues;
using BugTracking.Members;
using BugTracking.Follows;
using BugTracking.Notifications;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Users;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using BugTracking.Views;
using BugTracking.Azures;
using BugTracking.Departments;
using BugTracking.Teams;
using BugTracking.MemberTeams;
using BugTracking.TimeOnProjects;

namespace BugTracking.EntityFrameworkCore
{
    public static class BugTrackingDbContextModelCreatingExtensions
    {
        public static void ConfigureBugTracking(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(BugTrackingConsts.DbTablePrefix + "YourEntities", BugTrackingConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});

            builder.Entity<UserInforTFS.UserInforTfs>(b =>

            {
                b.ToTable(BugTrackingConsts.DbTablePrefix + "UserInforTfs", BugTrackingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
            });

            builder.Entity<TimeOnProject>(b =>

            {
                b.ToTable(BugTrackingConsts.DbTablePrefix + "TimeOnProject", BugTrackingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
            });

            builder.Entity<Conversation.Conversation>(b =>

            {
                b.ToTable(BugTrackingConsts.DbTablePrefix + "Conversations", BugTrackingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
            });
            builder.Entity<Azure>(b =>

            {
                b.ToTable(BugTrackingConsts.DbTablePrefix + "Azures", BugTrackingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
            });
            builder.Entity<Attachment>(b =>

            {
                b.ToTable(BugTrackingConsts.DbTablePrefix + "Attachments", BugTrackingConsts.DbSchema);
                b.Property(x => x.URL).IsRequired();
                b.Property(x => x.TableID).IsRequired();
                b.ConfigureByConvention(); //auto configure for the base class props
            });
            builder.Entity<Assignee>(b =>
            {
                b.ToTable(BugTrackingConsts.DbTablePrefix + "Assignees", BugTrackingConsts.DbSchema);
                b.ConfigureFullAuditedAggregateRoot();
                b.ConfigureByConvention(); //auto configure for the base class props

                b.HasOne<Issue>().WithMany().HasForeignKey(x => x.IssueID).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });
            builder.Entity<Categories.Category>(b =>
            {
                b.ConfigureFullAuditedAggregateRoot();

                b.ToTable(BugTrackingConsts.DbTablePrefix + "Categories", BugTrackingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(x => x.Name).IsRequired().HasMaxLength(500); ;
            });
            builder.Entity<Comment>(b =>
            {
                b.ConfigureFullAuditedAggregateRoot();

                b.ToTable(BugTrackingConsts.DbTablePrefix + "Comments", BugTrackingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props

                b.HasOne<Issue>().WithMany().HasForeignKey(x => x.IssueID).IsRequired().OnDelete(DeleteBehavior.NoAction);
                b.Property(x => x.Content).IsRequired().HasMaxLength(3000); ;
            });
            builder.Entity<DetailAttachment>(b =>
            {
                b.ToTable(BugTrackingConsts.DbTablePrefix + "DetailAttachments", BugTrackingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.HasOne<Attachment>().WithMany().HasForeignKey(x => x.AttachmentID).IsRequired().OnDelete(DeleteBehavior.NoAction);

            });
            builder.Entity<Issue>(b =>
            {

                b.ConfigureFullAuditedAggregateRoot();
                b.ToTable(BugTrackingConsts.DbTablePrefix + "Issues", BugTrackingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectID).IsRequired().OnDelete(DeleteBehavior.NoAction);
                b.HasOne<Categories.Category>().WithMany().HasForeignKey(x => x.CategoryID).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<Statuss.Status>().WithMany().HasForeignKey(x => x.StatusID).IsRequired().OnDelete(DeleteBehavior.NoAction);
                b.Property(x => x.Description).IsRequired(false).HasMaxLength(3000);
                b.Property(x => x.Priority).IsRequired().HasMaxLength(100);
                b.Property(x => x.IssueLevel).IsRequired().HasMaxLength(100);
                b.Property(x => x.Name).IsRequired().HasMaxLength(500);
            });
            builder.Entity<Member>(b =>
            {
                b.ConfigureFullAuditedAggregateRoot();
                b.ToTable(BugTrackingConsts.DbTablePrefix + "Members", BugTrackingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectID).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });
            builder.Entity<Notification>(b =>
            {
                b.ToTable(BugTrackingConsts.DbTablePrefix + "Notifications", BugTrackingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(x => x.Message).IsRequired().HasMaxLength(3000); ;
            });
            builder.Entity<Project>(b =>
            {
                b.ConfigureFullAuditedAggregateRoot();
                b.ToTable(BugTrackingConsts.DbTablePrefix + "Projects", BugTrackingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(x => x.Name).IsRequired().HasMaxLength(500);
            });
            builder.Entity<Statuss.Status>(b =>
            {
                b.ConfigureFullAuditedAggregateRoot();
                b.ToTable(BugTrackingConsts.DbTablePrefix + "Status", BugTrackingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(x => x.Name).IsRequired().HasMaxLength(500); ;
            });
            builder.Entity<Follow>(b =>
            {
                b.ToTable(BugTrackingConsts.DbTablePrefix + "Follows", BugTrackingConsts.DbSchema);
                b.ConfigureFullAuditedAggregateRoot();
                b.ConfigureByConvention(); //auto configure for the base class props
                b.HasOne<Issue>().WithMany().HasForeignKey(x => x.IssueID).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });
            builder.Entity<Department>(b =>
            {
                b.ToTable(BugTrackingConsts.DbTablePrefix + "Departments", BugTrackingConsts.DbSchema);
                b.ConfigureFullAuditedAggregateRoot();
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(x => x.Name).IsRequired().HasMaxLength(500);
            });
            builder.Entity<Team>(b =>
            {
                b.ToTable(BugTrackingConsts.DbTablePrefix + "Teams", BugTrackingConsts.DbSchema);
                b.ConfigureFullAuditedAggregateRoot();
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(x => x.Name).IsRequired().HasMaxLength(500);
                b.HasOne<Department>().WithMany().HasForeignKey(x => x.IdDepartment).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });
            builder.Entity<MemberTeam>(b =>
            {
                b.ConfigureFullAuditedAggregateRoot();
                b.ToTable(BugTrackingConsts.DbTablePrefix + "MemberTeams", BugTrackingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.HasOne<Team>().WithMany().HasForeignKey(x => x.IdTeam).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}