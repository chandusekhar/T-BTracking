using BugTracking.Assignees;
using BugTracking.Issues;
using BugTracking.Members;
using BugTracking.Projects;
using BugTracking.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace BugTracking.UserOData
{
    [RemoteService(IsEnabled = false)]
    public class UserODataAppService : BugTrackingAppService,IUserODataAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IAssigneeRepository _assineeRepository;
        public UserODataAppService(
            IUserRepository userRepository,
            IMemberRepository memberRepository,
            IProjectRepository projectRepository,
            IIssueRepository issueRepository,
            IAssigneeRepository assineeRepository
            )
        {
            _userRepository = userRepository;
            _memberRepository = memberRepository;
            _projectRepository = projectRepository;
            _issueRepository = issueRepository;
            _assineeRepository = assineeRepository;
        }
        public async Task<IQueryable<UserDto>> GetListUserByProject(Guid idProject)
        {
            IQueryable<AppUser> query = await _userRepository.GetQueryableAsync();
            var members = from user in query.AsNoTracking()
                          join member in _memberRepository on user.Id equals member.UserID
                          where member.ProjectID == idProject
                          select new UserDto
                          {
                             Name = user.Name,
                             Id = user.Id,
                             Email = user.Email,
                             PhoneNumber = user.PhoneNumber,
                             CreationTime = member.CreationTime,
                             IDMember = member.Id,
                             CreatedBy = member.CreatorId != null ? _userRepository.FirstOrDefault(x => x.Id == member.CreatorId).Name : "",
                             CountIssue = ((from issue in _issueRepository
                                             where issue.CreatorId == user.Id && issue.ProjectID == idProject
                                            select new { issue }).Count() + (from issue in _issueRepository
                                                                              join assign in _assineeRepository on issue.Id equals assign.IssueID
                                                                              where assign.UserID != issue.CreatorId && assign.UserID == user.Id && issue.ProjectID == idProject
                                                                             select new { issue }).Count()),
                              CountIssueFinish = (from issue in _issueRepository
                                                  where issue.CreatorId == user.Id && issue.FinishDate != null
                                                  select new { issue }).Count() + (from issue in _issueRepository
                                                                                   join assign in _assineeRepository on issue.Id equals assign.IssueID
                                                                                   where issue.CreatorId != assign.UserID && assign.UserID == user.Id && issue.FinishDate != null && issue.ProjectID == idProject
                                                                                   select new { issue }).Count(),
                          };
            return members;
        }
    }
}
