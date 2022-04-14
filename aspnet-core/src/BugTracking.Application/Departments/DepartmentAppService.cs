using BugTracking.Assignees;
using BugTracking.Categories;
using BugTracking.IShareDto;
using BugTracking.Issues;
using BugTracking.MemberTeams;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Teams;
using BugTracking.Users;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Departments
{
    [RemoteService(IsEnabled = false)]
   // [Authorize("BugTracking.Users")]
    public class DepartmentAppService : BugTrackingAppService, IDepartmentAppService
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
        public DepartmentAppService(
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
        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
        {

            if (_departmentRepository.Any(x => x.Name.ToLower().Trim() == input.Name.ToLower().Trim()))
            {
                throw new UserFriendlyException("Department Name Exist! Try Another Name!");
            }
            if (input.Name.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Input name do not null! Try Another Name!");
            }
            var department = _departmentManager.CreateAsync(input.Name.Trim(), input.IdManager.Trim());
            await _departmentRepository.InsertAsync(department);
            return ObjectMapper.Map<Department, DepartmentDto>(department);
        }
        public async Task UpdateAsync(Guid id, UpdateDepartmentDto input)
        {
            var DepartmentList = await _departmentRepository.GetListAsync(x => x.Id != id);
            if (DepartmentList.Any(x => x.Name.ToLower().Trim() == input.Name.ToLower().Trim()))
            {
                throw new UserFriendlyException("Department Name Exist! Try Another Name!");
            }
            var department = await _departmentRepository.GetAsync(id);
            department.Name = input.Name.Trim();
            department.IdManager = input.IdManager;
            await _departmentRepository.UpdateAsync(department);
        }
        public async Task<ListResultDto<DepartmentDto>> GetListAsync()
        {
            var departments = await _departmentRepository.GetListAsync();

            return new ListResultDto<DepartmentDto>(
               ObjectMapper.Map<List<Department>, List<DepartmentDto>>(departments)
           );
        }

        public async Task<List<DepartmentDto>> GetListDepartment(string filter)
        {
            if (filter.IsNullOrWhiteSpace() || filter == "null")
            {
                filter = "";
            }
            IQueryable<Department> departmentQueryable = await _departmentRepository.GetQueryableAsync();
            var query = from department in departmentQueryable
                        where department.Name.Contains(filter)
                        orderby department.CreationTime descending
                        select new
                        { department };
            var queryResult = await AsyncExecuter.ToListAsync(query);
            var departmentDtos = queryResult.Select(result =>
            {
                var DepartmentDto = ObjectMapper.Map<Department, DepartmentDto>(result.department);
                var manager = _userRepository.Where(u => u.Id == result.department.IdManager).FirstOrDefault();
                DepartmentDto.NameManager = manager.Name;
                int countIssueCreate = 0;
                int countIssueAssign = 0;
                var teams = _teamRepository.Where(x => x.IdDepartment == result.department.Id).ToList();
                DepartmentDto.CountMember = ((from team in _teamRepository
                                              join memberTeam in _memberTeamReoprository on team.Id equals memberTeam.IdTeam
                                              where team.IdDepartment == result.department.Id
                                              select new { memberTeam }).Count());
                countIssueCreate = ((from Team in _teamRepository
                                     join memberTeam in _memberTeamReoprository on Team.Id equals memberTeam.IdTeam into termMember
                                     from m in termMember.DefaultIfEmpty()
                                     join issue in _issueRepository on m.UserID equals issue.CreatorId
                                     where Team.IdDepartment == result.department.Id
                                     select new { issue }).Count());
                countIssueAssign = ((from Team in _teamRepository
                                     join memberTeam in _memberTeamReoprository on Team.Id equals memberTeam.IdTeam into termMember
                                     from m in termMember.DefaultIfEmpty()
                                     join assign in _assineeRepository on m.UserID equals assign.UserID into termAssign
                                     from a in termAssign.DefaultIfEmpty()
                                     join issue in _issueRepository on a.IssueID equals issue.Id
                                     where Team.IdDepartment == result.department.Id
                                     select new { issue }).Count());
                DepartmentDto.CountIssue = countIssueCreate + countIssueAssign;
                return DepartmentDto;
            }).ToList();
            return departmentDtos;


        }
        public async Task DeleteAsync(Guid id)
        {

            var department = await _departmentRepository.GetAsync(id);
            var team = _teamRepository.Where(x => x.IdDepartment == id).ToList();
            for (int i = 0; i < team.Count(); i++)
            {
                var memberteam = _memberTeamReoprository.Where(x => x.IdTeam == team[i].Id).ToList();
                if (memberteam != null)
                {
                    for (int j = 0; j < memberteam.Count(); j++)
                    {
                        await _memberTeamReoprository.DeleteAsync(memberteam[i].Id);
                    }
                }
                await _teamRepository.DeleteAsync(team[i].Id);
            }
            await _departmentRepository.DeleteAsync(id);
        }
        public class IssueDtoCompare : IEqualityComparer<IssuesDto>
        {
            public bool Equals(IssuesDto x, IssuesDto y)
            {
                if (object.ReferenceEquals(x, y))
                {
                    return true;
                }

                //If either one of the object refernce is null, return false
                if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                {
                    return false;
                }

                //Comparing all the properties one by one
                return x.Id == y.Id;
            }

            public int GetHashCode([DisallowNull] IssuesDto obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                //Get the ID hash code value
                int IDHashCode = obj.Id.GetHashCode();

                //Get the string HashCode Value
                //Check for null refernece exception

                return IDHashCode;
            }
        }
        // Lấy danh danh sách issue trong form department

        public async Task<PagedResultDto<IssuesDto>> GetListIssueAll(GetListDto input, string IdProject, string IdStatus, string IdCate, string IdUser,string IdDepartment,string IdTeam,bool IsAss)
        {
            if (input.Filter.IsNullOrWhiteSpace() || input.Filter == "null")
            {
                input.Filter = "";
            }
            if (IdUser.IsNullOrWhiteSpace() || IdUser == "null")
            {
                IdUser = "";
            }
            if (IdProject.IsNullOrWhiteSpace() || IdProject == "null")
            {
                IdProject = "";
            }
            if (IdStatus.IsNullOrWhiteSpace() || IdStatus == "null")
            {
                IdStatus = "";
            }
            if (IdCate.IsNullOrWhiteSpace() || IdCate == "null")
            {
                IdCate = "";
            }
           
            if (IdTeam.IsNullOrWhiteSpace() || IdTeam == "null")
            {
                IdTeam = "";
            }
            if (IdDepartment.IsNullOrWhiteSpace() || IdDepartment == "null")
            {
                IdDepartment = "";
            }
                    // get list issue for member assign
                    IssueDtoCompare issueDtoCompare = new IssueDtoCompare();
                    var queryable = await _assineeRepository.GetQueryableAsync();
            var ass = from issue in _issueRepository
                      join assign in _assineeRepository on issue.Id equals assign.IssueID into termAssign
                      from a in termAssign.DefaultIfEmpty()
                      join status in _statusRepository on issue.StatusID equals status.Id
                      join cat in _categoryRepository on issue.CategoryID equals cat.Id into termCategory
                      from c in termCategory.DefaultIfEmpty()
                      join project in _projectRepository on issue.ProjectID equals project.Id
                      join user in _userRepository on issue.CreatorId equals user.Id
                      join memberteam in _memberTeamReoprository on a.UserID equals memberteam.UserID
                      join team in _teamRepository on memberteam.IdTeam equals team.Id into termTeam
                      from t in termTeam.DefaultIfEmpty()
                      join department in _departmentRepository on t.IdDepartment equals department.Id
                      where issue.Name.Contains(input.Filter) && issue.CreatorId.Contains(IdUser)
                              && issue.ProjectID.ToString().Contains(IdProject)
                              && issue.StatusID.ToString().Contains(IdStatus)
                              && issue.CategoryID.ToString().Contains(IdCate)
                              && t.Id.ToString().Contains(IdTeam)
                              && department.Id.ToString().Contains(IdDepartment)
                      orderby issue.CreationTime descending
                      select new IssuesDto()
                      {
                          UserID = issue.CreatorId,
                          UserName = user == null ? null : user.Name,
                          StatusID = issue.StatusID,
                          StatusName = status == null ? null : status.Name,
                          ProjectID = issue.ProjectID,
                          ProjectName = project == null ? null : project.Name,
                          CategoryID = issue.CategoryID,
                          CategoryName = c == null ? null : c.Name,
                          DueDate = issue.DueDate,
                          CreationTime = issue.CreationTime,
                          FinishDate = issue.FinishDate,
                          Id = issue.Id,
                          Name = issue.Name,
                          NzColor = status.NzColor,
                      };
            var cre = from issue in _issueRepository
                        join status in _statusRepository on issue.StatusID equals status.Id
                        join cat in _categoryRepository on issue.CategoryID equals cat.Id into termCategory
                        from c in termCategory.DefaultIfEmpty()
                        join project in _projectRepository on issue.ProjectID equals project.Id
                        join user in _userRepository on issue.CreatorId equals user.Id 
                        join memberteam in _memberTeamReoprository on issue.CreatorId equals memberteam.UserID
                        join team in _teamRepository on memberteam.IdTeam equals team.Id
                        join department in _departmentRepository on team.IdDepartment equals department.Id
                        where issue.Name.Contains(input.Filter) && issue.CreatorId.Contains(IdUser)
                                && issue.ProjectID.ToString().Contains(IdProject)
                                && issue.StatusID.ToString().Contains(IdStatus)
                                && issue.CategoryID.ToString().Contains(IdCate)
                                && team.Id.ToString().Contains(IdTeam)
                                && department.Id.ToString().Contains(IdDepartment)
                      select new IssuesDto()
                      {
                          UserID = issue.CreatorId,
                          UserName = user == null ? null : user.Name,
                          StatusID = issue.StatusID,
                          StatusName = status == null ? null : status.Name,
                          ProjectID = issue.ProjectID,
                          ProjectName = project == null ? null : project.Name,
                          CategoryID = issue.CategoryID,
                          CategoryName = c == null ? null : c.Name,
                          DueDate = issue.DueDate,
                          CreationTime = issue.CreationTime,
                          FinishDate = issue.FinishDate,
                          Id = issue.Id,
                          Name = issue.Name,
                          NzColor = status.NzColor,
                      };
            var queryResult = (IsAss) ? ass : cre;
            var isu = queryResult.Distinct();
            var totalCount = isu.Count();
            isu = isu.Skip(input.SkipCount).Take(input.MaxResultCount);
            var result = isu.Distinct().ToList();
            return new PagedResultDto<IssuesDto>(
                totalCount,
                result
            );   
        }
        public async Task<List<IssuesDto>> GetListIssueById(Guid IdDepartment)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            bool isAdmin = await _userManager.CheckRoleByUserId(CurrentUserId, "admin");
            if (isAdmin)
            {

            }
            var queryable = await _assineeRepository.GetQueryableAsync();
            var query = from assign in queryable
                        join issue in _issueRepository on assign.IssueID equals issue.Id
                        into termIssue
                        from i in termIssue.DefaultIfEmpty()
                        join memberteam in _memberTeamReoprository on i.CreatorId equals memberteam.UserID into termMemberTeam
                        from m in termMemberTeam.DefaultIfEmpty()
                        join team in _teamRepository on m.IdTeam equals team.Id into termTeam
                        from t in termTeam.DefaultIfEmpty()
                        join department in _departmentRepository on t.IdDepartment equals department.Id
                        where department.Id == IdDepartment
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
        // check manager
   
        public bool GetCheckManager(string UserId)
        {
           // var CurrentUserID = _tmtCurrentUser.GetId();
            return _departmentRepository.Any(x => x.IdManager == UserId);
                 
        }
        // get id Department by id Manager
        public List<string>getNameDepartmentByManager(string IdUser)
        {
            List<string> requests = new List<string>();
            string id = _departmentRepository.FirstOrDefault(x => x.IdManager == IdUser).Id.ToString();
            string name = _departmentRepository.FirstOrDefault(x => x.IdManager == IdUser).Name;
            requests.Add(id);
            requests.Add(name);
            return requests;
        }
    }
}
