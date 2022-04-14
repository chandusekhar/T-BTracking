using BugTracking.Admin;
using BugTracking.Assignees;
using BugTracking.Categories;
using BugTracking.Issues;
using BugTracking.Members;
using BugTracking.ODatas;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AuditLogging;

namespace BugTracking.AdminOData
{
    // [Authorize("BugTracking.Users")]
   [RemoteService(IsEnabled = false)]
    public class AdminODataAppService : BugTrackingAppService, IAdminODataAppService
    {

        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IAssigneeRepository _assignRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly ICategoryRepository _categoryRepository;

        public AdminODataAppService(
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            IIssueRepository issueRepository,
            IAssigneeRepository assignRepository,
            IMemberRepository memberRepository,
            IStatusRepository statusRepository,
            ICategoryRepository categoryRepository
            )
        {
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _issueRepository = issueRepository;
            _assignRepository = assignRepository;
            _memberRepository = memberRepository;
            _statusRepository = statusRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<IQueryable<AdminODataDto>> GetUserAdmin()
        {
            IQueryable<AppUser> query = await _userRepository.GetQueryableAsync();
            var users = from user in query.AsNoTracking()
                        select new AdminODataDto
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber.Replace("+84","0"),
                            ProjectCreate = _projectRepository.Count(x => x.CreatorId == user.Id),
                            IssueCreated = _issueRepository.Count(x => x.CreatorId == user.Id),
                            CountIssueAssign = _assignRepository.Count(x => x.UserID == user.Id),
                            TotalIssue = (from issue in _issueRepository
                                          join assignee in _assignRepository on issue.Id equals assignee.IssueID
                                          where assignee.UserID != issue.CreatorId && assignee.UserID == user.Id
                                          select new { issue }).Count() + _issueRepository.Count(x => x.CreatorId == user.Id),
                            IssueDueDate = (from issue in _issueRepository
                                            join assignee in _assignRepository on issue.Id equals assignee.IssueID
                                            where assignee.UserID != issue.CreatorId && assignee.UserID == user.Id && issue.DueDate <= DateTime.Today
                                            select new { issue }).Count() + _issueRepository.Count(x => x.CreatorId == user.Id && x.DueDate <= DateTime.Today),
                            IssueFinish = (from issue in _issueRepository
                                           join assignee in _assignRepository on issue.Id equals assignee.IssueID
                                           where assignee.UserID != issue.CreatorId && assignee.UserID == user.Id && issue.FinishDate != null
                                           select new { issue }).Count() + _issueRepository.Count(x => x.CreatorId == user.Id && x.DueDate != null),
                        };
            return users;
        }
        public async Task<IQueryable<ProjectDto>> GetListProject()
        {
            //var bd
            IQueryable<Project> query = await _projectRepository.GetQueryableAsync();
            var projects = from project in query.AsNoTracking()
                           join user in _userRepository on project.CreatorId equals user.Id
                           select new ProjectDto
                           {
                               Id = project.Id,
                               Name = project.Name,
                               UserId = user.Id,
                               userName = user.Name,
                               CreationTime = project.CreationTime,
                               CountMember = _memberRepository.Where(x => x.ProjectID == project.Id).Count(),
                               countIssue = _issueRepository.Where(x => x.ProjectID == project.Id).Count(),
                               countIssueDueDate = _issueRepository.Where(x => x.ProjectID == project.Id && x.DueDate < DateTime.Today).Count(),
                               countIssueClose = _issueRepository.Where(x => x.ProjectID == project.Id && x.FinishDate != null).Count()
                           };
            return projects;
        }
        public async Task<IQueryable<IssueAdminODataDto>> GetListIssue(string idUserAssign)
        {
            if (idUserAssign.IsNullOrWhiteSpace() || idUserAssign == "null")
            {
                idUserAssign = "";
            }
            IQueryable<Issue> query = await _issueRepository.GetQueryableAsync();
                var issues = from issue in query.AsNoTracking()
                             join status in _statusRepository on issue.StatusID equals status.Id
                             join project in _projectRepository on issue.ProjectID equals project.Id
                             join assign in _assignRepository on issue.Id equals assign.IssueID into termAssign
                             from a in termAssign.DefaultIfEmpty()
                             join cat in _categoryRepository on issue.CategoryID equals cat.Id into termCategory
                             from c in termCategory.DefaultIfEmpty()
                             join user in _userRepository on issue.CreatorId equals user.Id
                             join user1 in _userRepository on a.UserID equals user1.Id into termUserAssign
                             from u in termUserAssign.DefaultIfEmpty()
                             where a.UserID.Contains(idUserAssign)
                             select new IssueAdminODataDto
                             {
                                 IdIssue = issue.Id,
                                 Name = issue.Name,
                                 UserID = user.Id,
                                 UserName = user.Name,
                                 UserNameAssign = u.Name,
                                 ProjectID = project.Id,
                                 ProjectName = project.Name,
                                 StatusID = status.Id,
                                 CategoryID = c == null ? null : c.Id,
                                 StatusName = status.Name,
                                 CategoryName = c == null ? null : c.Name,
                                 CountIssueDueDate = _issueRepository.Where(x => x.DueDate < DateTime.Today).Count(),
                                 StartDate = issue.CreationTime,
                                 DueDate = issue.DueDate,
                                 NzColor = status.NzColor,
                                 FinishDate = issue.FinishDate,
                             };
            return issues;
        }
        //public List<string> ListUserAssign(Guid idIssue)
        //{
        //    var a = new List<string> { };
        //    foreach (Assignee assign in _assignRepository.Where(x => x.IssueID == idIssue))
        //    {
        //        string UserName = _userRepository.FirstOrDefault(x => x.Id == assign.UserID).Name;
        //        a.Add(UserName);
        //    }
        //    return a;
        //}
    }
}
