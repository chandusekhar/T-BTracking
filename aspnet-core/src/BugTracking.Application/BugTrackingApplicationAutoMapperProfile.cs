using AutoMapper;
using BugTracking.Assignees;
using BugTracking.Attachments;
using BugTracking.Categories;
using BugTracking.Comments;
using BugTracking.Conversation;
using BugTracking.Departments;
using BugTracking.Histories;
using BugTracking.HistoryDashboards;
using BugTracking.Issues;
using BugTracking.Members;
using BugTracking.MemberTeams;
using BugTracking.Notifications;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Teams;
using BugTracking.TimeOnProjects;
using BugTracking.Users;

namespace BugTracking
{
    public class BugTrackingApplicationAutoMapperProfile : Profile
    {
        public BugTrackingApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */
            CreateMap<Category, CategoryDto>();
            CreateMap<Category, CategoryBoardDto>();
            CreateMap<Status, StatusDTO>();
            CreateMap<Project, ProjectDto>();
            CreateMap<Issue, IssuesDto>();
            CreateMap<Issue, CreateUpdateIssuesDto>();
            CreateMap<Category, CategoryLookupDto>();
            CreateMap<Status, StatusLookupDto>();
            CreateMap<Project, ProjectLookupDto>();
            CreateMap<AppUser, UserLookupDto>();
            CreateMap<Assignee, AssigneeDto>();
            CreateMap<Attachment, AttachmentDto>();
            CreateMap<Comment, CommentDto>();
            CreateMap<Department, DepartmentDto>();
            CreateMap<Team, TeamDto>();
            CreateMap<MemberTeam, MemberTeamDto>();
            CreateMap<Member, MemberDto>();
            CreateMap<Notification, NotificationDto>();
            CreateMap<Notification, MessageNotifyDto>();
            CreateMap<DetailAttachments.DetailAttachment, DetailAttachment.DetailAttachmentDto>();
            CreateMap<AppUser, UserDto>();
            CreateMap<Volo.Abp.AuditLogging.AuditLog, HistoryDTO>();
            CreateMap<Status, StatusContractDto>();
            CreateMap<Azures.Azure, Azures.AzureDto>();
            CreateMap<Follows.Follow, Follows.FollowDto>();
            CreateMap<Conversation.Conversation, ConversationDTO>();
            CreateMap<UserInforTFS.UserInforTfs, UserInforTfs.UserInforTfsDto>();
            CreateMap<Issue, IssuesNoParentDto>();
            CreateMap<TimeOnProject, TimeOnProjectDto>();
            CreateMap<Status, StatusStatisticsDto>();
            CreateMap<Assignee, UserAssigneesDto>();
            CreateMap<Member, Member1StatisticDto>();
            CreateMap<Project, ProjectTfsDto>();
            CreateMap<Member, MemberStatisticsDto>();
            CreateMap<HistoryViews.HistoryView, HistoryViewDto>();
            CreateMap<Issue, IssueDetailCalendar>();
            CreateMap<HistoryDashboard, HistoryDashboardDto>();
            CreateMap<Project, TagProjectDTO>();
        }
    }
}
