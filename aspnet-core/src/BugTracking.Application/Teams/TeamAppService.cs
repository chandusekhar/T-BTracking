/// Service quản lý team
/// Quản lý thêm/sửa/xóa/các nghiệp vụ liên quan đến team
/// (c) 2021 Bởi Đỗ Vạn Thành

using BugTracking.Assignees;
using BugTracking.Categories;
using BugTracking.Departments;
using BugTracking.Issues;
using BugTracking.MemberTeams;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Users;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;

namespace BugTracking.Teams
{
    [Authorize("BugTracking.Users")]
    public class TeamAppService : BugTrackingAppService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly TeamManager _teamManager;
        private readonly IMemberTeamRepository _memberTeamRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IAssigneeRepository _assineeRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager _userManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly MemberTeamManager _memberTeamManager;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly IDepartmentRepository _departmentRepository;
        public TeamAppService(
            ITeamRepository teamRepository,
            TeamManager teamManager,
            IMemberTeamRepository memberTeamRepository,
             IIssueRepository issueRepository,
            IAssigneeRepository assineeRepository,
            IStatusRepository statusRepository,
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            ICategoryRepository categoryRepository,
            UserManager userManager,
            MemberTeamManager memberTeamManager,
            ITMTCurrentUser tmtCurrentUser,
            IDepartmentRepository departmentRepository
            )
        {
            _teamRepository = teamRepository;
            _teamManager = teamManager;
            _memberTeamRepository = memberTeamRepository;
            _issueRepository = issueRepository;
            _assineeRepository = assineeRepository;
            _statusRepository = statusRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            _memberTeamManager = memberTeamManager;
            _tmtCurrentUser = tmtCurrentUser;
            _departmentRepository = departmentRepository;
        }
        public async Task<TeamDto> CreateAsync(CreateTeamDto input)
        {

            if (_teamRepository.Any(x => x.Name.ToLower().Trim() == input.Name.ToLower().Trim()))
            {
                throw new UserFriendlyException("Department Name Exist! Try Another Name!");
            }
            if (input.Name.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Input name do not null! Try Another Name!");
            }
            var team = _teamManager.CreateAsync(input.Name.Trim(), input.IdLeader.Trim(),input.IdDepartment);
            await _teamRepository.InsertAsync(team);
            var merberTeam = await _memberTeamManager.CreateAsync(team.Id, input.IdLeader);
            await _memberTeamRepository.InsertAsync(merberTeam);
            return ObjectMapper.Map<Team, TeamDto>(team);
        }
        public async Task UpdateAsync(Guid id, UpdateTeamDto input)
        {
            var teamList = await _teamRepository.GetListAsync(x => x.Id != id);
            if (teamList.Any(x => x.Name.ToLower().Trim() == input.Name.ToLower().Trim()))
            {
                throw new UserFriendlyException("Team Name Exist! Try Another Name!");
            }
            var team = await _teamRepository.GetAsync(id);
            var memberteamDL = _memberTeamRepository.Where(x=>x.IdTeam==id && x.UserID==team.IdLeader).FirstOrDefault();
            await _memberTeamRepository.DeleteAsync(memberteamDL.Id);
            team.Name = input.Name.Trim();
            team.IdLeader = input.IdLeader;
            await _teamRepository.UpdateAsync(team);
            var merberTeam = await _memberTeamManager.CreateAsync(id, input.IdLeader);
            await _memberTeamRepository.InsertAsync(merberTeam);
        }
        public async Task DeleteAsync(Guid id)
        {

            var team = await _teamRepository.GetAsync(id);
            var memberteam = _memberTeamRepository.Where(x => x.IdTeam == id).ToList();
           foreach(MemberTeam member in memberteam)
            {
                await _memberTeamRepository.DeleteAsync(member.Id);
            }
            await _teamRepository.DeleteAsync(id);
        }
        // Lấy danh sách team theo user đăng nhập và tìm kiếm theo department
        public async Task<List<TeamDto>> GetListTeam(string input,string DepartmentId)
        {
            //if (UserId.IsNullOrWhiteSpace() || UserId == "null")
            //{
            //    UserId = "";
            //}
            if (input.IsNullOrWhiteSpace() || input == "null")
            {
                input = "";
            }
            if (DepartmentId.IsNullOrWhiteSpace() || DepartmentId == "null")
            {
                DepartmentId = "";
            }
            //if (await _userManager.CheckRoleByUserId(UserId, "admin"))
            //{
            var queryable = await _teamRepository.GetQueryableAsync();
            var query = from team in queryable
                        where team.Name.Contains(input) && team.IdDepartment.ToString().Contains(DepartmentId)
                        select new { team };
            var queryResult = await AsyncExecuter.ToListAsync(query);
            int countIssueCreate = 0;
            int countIssueAssign = 0;
            var teamDtos = queryResult.Select(result =>
            {
                var teamDto = ObjectMapper.Map<Team, TeamDto>(result.team);
                var leader = _userRepository.Where(u => u.Id == result.team.IdLeader).FirstOrDefault();
                teamDto.NameLeader = leader.Name;
                var members = _memberTeamRepository.Where(x => x.IdTeam == result.team.Id).ToList();
                teamDto.CountMember = members.Count();
                var d = _departmentRepository.GetAsync(result.team.IdDepartment);
                var department = ObjectMapper.Map<Department, DepartmentDto>(d.Result);
                teamDto.NameDepartment = department.Name;
                countIssueCreate = ((from member in _memberTeamRepository
                                     join issue in _issueRepository on member.UserID equals issue.CreatorId
                                     where member.IdTeam == result.team.Id
                                     select new { issue }).Count());
                countIssueAssign = ((from member in _memberTeamRepository
                                     join assign in _assineeRepository on member.UserID equals assign.UserID into termAssign
                                     from a in termAssign.DefaultIfEmpty()
                                     join issue in _issueRepository on a.IssueID equals issue.Id
                                     where member.IdTeam == result.team.Id
                                     select new { issue }).Count());
                teamDto.CountIssue = countIssueCreate + countIssueAssign;
                return teamDto;
            }).ToList();
            return teamDtos;
            //  }
            //else
            //{ 
            //    var queryable = await _teamRepository.GetQueryableAsync();
            //    var query = from team in queryable
            //                where team.Name.Contains(input) && team.IdDepartment.ToString() == DepartmentId
            //                select new { team };
            //    var queryResult = await AsyncExecuter.ToListAsync(query);
            //    var teamDtos = queryResult.Select(result =>
            //    {
            //        var teamDto = ObjectMapper.Map<Team, TeamDto>(result.team);
            //        var leader = _userRepository.Where(u => u.Id == result.team.IdLeader).FirstOrDefault();
            //        teamDto.NameLeader = leader.Name;
            //        var members = _memberTeamRepository.Where(x => x.IdTeam == result.team.Id).ToList();
            //        teamDto.CountMember = members.Count();
            //        var d = _departmentRepository.GetAsync(result.team.IdDepartment);
            //        var department = ObjectMapper.Map<Department, DepartmentDto>(d.Result);
            //        teamDto.NameDepartment = department.Name;
            //        return teamDto;
            //    }).ToList();
            //    return teamDtos;
            //}
        }
        // Lấy danh sách issue theo team
        public async Task<List<IssuesDto>> GetListIssueById(Guid IdTeam)
        {
            var queryable = await _assineeRepository.GetQueryableAsync();
            var query = from assign in queryable
                        join issue in _issueRepository on assign.IssueID equals issue.Id
                        into termIssue
                        from i in termIssue.DefaultIfEmpty()
                        join memberteam in _memberTeamRepository on i.CreatorId equals memberteam.UserID into termMemberTeam
                        from m in termMemberTeam.DefaultIfEmpty()
                        join team in _teamRepository on m.IdTeam equals team.Id
                        where team.Id == IdTeam
                        orderby i.CreationTime descending
                        select new
                        {
                            i

                        };
            var queryResult = await AsyncExecuter.ToListAsync(query);

            var issueDtos = queryResult.Select(x =>
            {
                var issueDto = ObjectMapper.Map<Issue, IssuesDto>(x.i);
                var a = _userRepository.GetAsync(x.i.CreatorId);
                var userCreate = ObjectMapper.Map<AppUser, Users.UserDto>(a.Result);
                issueDto.UserName = userCreate.Name;
                var s = _statusRepository.GetAsync(x.i.StatusID);
                var status = ObjectMapper.Map<Status, StatusDTO>(s.Result);
                issueDto.StatusName = status.Name;
                var p = _projectRepository.GetAsync(x.i.ProjectID);
                var project = ObjectMapper.Map<Project, ProjectDto>(p.Result);
                issueDto.ProjectName = project.Name;
                return issueDto;
            }).ToList();
            return issueDtos;
        }
        // check Leader 
        public bool GetCheckLeader(string UserId)
        {
          // var CurrentUserID = _tmtCurrentUser.GetId();
            return _teamRepository.Any(x => x.IdLeader == UserId);

        }
        // get Name Team by id Leader
        /// <summary>
        /// Get tên Team từ Id Leader
        /// </summary>
        /// <param name="IdUser"> Id Leader</param>
        /// <returns></returns>
        public List<string>getNameTeamByLeader(string IdUser)
        {
            List<string> requests = new List<string>();
            string name = _teamRepository.FirstOrDefault(x => x.IdLeader == IdUser).Name;
            string id = _teamRepository.FirstOrDefault(x => x.IdLeader == IdUser).Id.ToString();
            requests.Add(id);
            requests.Add(name);
            return requests;
          //  return _teamRepository.FirstOrDefault(x => x.IdLeader == IdUser).Name;

        }
    }
}
