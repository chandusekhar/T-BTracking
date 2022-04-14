using BugTracking.Assignees;
using BugTracking.Categories;
using BugTracking.Departments;
using BugTracking.Issues;
using BugTracking.MemberTeams;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Teams;
using BugTracking.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;

namespace BugTracking.DepartmentOData
{
    //[Authorize("BugTracking.Users")]
    [RemoteService(IsEnabled = false)]
    public class DepartmentODataAppService : BugTrackingAppService, IDepartmentODataAppService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly DepartmentManager _departmentManager;
        private readonly ITeamRepository _teamRepository;
        private readonly IMemberTeamRepository _memberTeamReoprository;
        private readonly IIssueRepository _issueRepository;
        private readonly IAssigneeRepository _assineeRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly UserManager _userManager;
        public DepartmentODataAppService(
            IDepartmentRepository departmentRepository,
            DepartmentManager departmentManager,
            ITeamRepository teamRepository,
            IMemberTeamRepository memberTeamReoprository,
            IIssueRepository issueRepository,
            IAssigneeRepository assineeRepository,
            IStatusRepository statusRepository,
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            ICategoryRepository categoryRepository,
            ITMTCurrentUser tmtCurrentUser,
            UserManager userManager
            )
        {
            _departmentRepository = departmentRepository;
            _departmentManager = departmentManager;
            _teamRepository = teamRepository;
            _memberTeamReoprository = memberTeamReoprository;
            _issueRepository = issueRepository;
            _assineeRepository = assineeRepository;
            _statusRepository = statusRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _tmtCurrentUser = tmtCurrentUser;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }
        public async Task<IQueryable<DepartmentODataDto>> GetListDepartment()
        {
            //var bd
            IQueryable<Department> query = await _departmentRepository.GetQueryableAsync();
            var departments = from department in query.AsNoTracking()
                              select new DepartmentODataDto
                              {
                                  Id = department.Id,
                                  Name = department.Name,
                                  NameManager = _userRepository.FirstOrDefault(x => x.Id == department.IdManager).Name,
                                  IdManager = department.IdManager,
                                  CountMember = (from team in _teamRepository
                                                 join memberTeam in _memberTeamReoprository on team.Id equals memberTeam.IdTeam
                                                 where team.IdDepartment == department.Id
                                                 select new { memberTeam }).Count(),
                                  CountIssue = ((from Team in _teamRepository
                                                 join memberTeam in _memberTeamReoprository on Team.Id equals memberTeam.IdTeam into termMember
                                                 from m in termMember.DefaultIfEmpty()
                                                 join issue in _issueRepository on m.UserID equals issue.CreatorId
                                                 where Team.IdDepartment == department.Id
                                                 select new { issue }).Count() + (from Team in _teamRepository
                                                                                  join memberTeam in _memberTeamReoprository on Team.Id equals memberTeam.IdTeam into termMember
                                                                                  from m in termMember.DefaultIfEmpty()
                                                                                  join assign in _assineeRepository on m.UserID equals assign.UserID into termAssign
                                                                                  from a in termAssign.DefaultIfEmpty()
                                                                                  join issue in _issueRepository on a.IssueID equals issue.Id
                                                                                  where Team.IdDepartment == department.Id && issue.CreatorId != a.UserID
                                                                                  select new { issue }).Count()),
                              };
            return departments;
        }
        public async Task<IQueryable<TeamDto>> GetListTeam()
        {
            IQueryable<Team> query = await _teamRepository.GetQueryableAsync();
            var teams = from team in query.AsNoTracking()
                        select new TeamDto
                        {
                            Id = team.Id,
                            Name = team.Name,
                            IdLeader = team.IdLeader,
                            NameLeader = _userRepository.FirstOrDefault(x => x.Id == team.IdLeader).Name,
                            CountMember = _memberTeamReoprository.Where(x => x.IdTeam == team.Id).Count(),
                            CountIssue = ((from member in _memberTeamReoprository
                                           join issue in _issueRepository on member.UserID equals issue.CreatorId
                                           where member.IdTeam == team.Id
                                           select new { issue }).Count() + (from member in _memberTeamReoprository
                                                                            join ass in _assineeRepository on member.UserID equals ass.UserID into termAssign
                                                                            from a in termAssign.DefaultIfEmpty()
                                                                            join issue in _issueRepository on a.IssueID equals issue.Id
                                                                            where a.UserID != issue.CreatorId && member.IdTeam == team.Id
                                                                            select new { issue }).Count()),
                            NameDepartment = _departmentRepository.FirstOrDefault(x=>x.Id == team.IdDepartment).Name,
                            IdDepartment = _departmentRepository.FirstOrDefault(x => x.Id == team.IdDepartment).Id,
                        };
            return teams;
        }
        public async Task<IQueryable<MemberTeamDto>> GetListMemberByTeam()
        {
            IQueryable<MemberTeam> query = await _memberTeamReoprository.GetQueryableAsync();
            var members = from member in query.AsNoTracking()
                          join team in _teamRepository on member.IdTeam equals team.Id
                        select new MemberTeamDto
                        {
                            Id = member.Id,
                            IdTeam = team.Id,
                            NameTeam = team.Name,
                            IdUser = _userRepository.FirstOrDefault(x => x.Id == member.UserID).Id,
                            NameMember = _userRepository.FirstOrDefault(x => x.Id == member.UserID).Name,
                            PhoneNumber = _userRepository.FirstOrDefault(x => x.Id == member.UserID).PhoneNumber,
                            Email = _userRepository.FirstOrDefault(x => x.Id == member.UserID).Email,
                            CountIssue = (from issue in _issueRepository
                                          where issue.CreatorId == member.UserID
                                          select new { issue }).Count() + (from issue in _issueRepository
                                                                           join assign in _assineeRepository on issue.Id equals assign.IssueID
                                                                           where assign.UserID != issue.CreatorId && assign.UserID == member.UserID
                                                                           select new { issue }).Count(),
                            
                        };
            return members;
        }
    }
}
