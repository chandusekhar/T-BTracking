using BugTracking.Assignees;
using BugTracking.Categories;
using BugTracking.Issues;
using BugTracking.Members;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Users;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AuditLogging;
using Volo.Abp.Domain.Entities;

namespace BugTracking.Admin
{
    [Authorize("BugTracking.Users")]
    public class AdminAppService : BugTrackingAppService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IAssigneeRepository _assignRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager _userManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStatusRepository _statusRepository;
        public AdminAppService(
            IUserRepository userRepository,
            IAuditLogRepository auditLogRepository,
            IAssigneeRepository assignRepository,
            IProjectRepository projectRepository,
            IIssueRepository issueRepository,
            UserManager userManager,
            IStatusRepository statusRepository,
            ICategoryRepository categoryRepository,
        IMemberRepository memberRepository)
        {
            _userRepository = userRepository;
            _auditLogRepository = auditLogRepository;
            _assignRepository = assignRepository;
            _memberRepository = memberRepository;
            _projectRepository = projectRepository;
            _issueRepository = issueRepository;
            _userManager = userManager;
            _statusRepository = statusRepository;
            _categoryRepository = categoryRepository;


        }
        public async Task<List<Users.UserDto>> GetListUser(string input)
        {
            if (input.IsNullOrWhiteSpace() || input == "null")
            {
                input = "";
            }
            var query = from user in _userRepository
                        where user.Name.Contains(input) || user.Email.Contains(input) || user.PhoneNumber.Contains(input)
                        orderby user.CreationTime descending
                        select new { user };

            var queryResult = await AsyncExecuter.ToListAsync(query);
            var UserDtos = queryResult.Select(result =>
            {
                var UserDto = ObjectMapper.Map<AppUser, Users.UserDto>(result.user);
                int CountIssueCreated = _issueRepository.Count(x => x.CreatorId == result.user.Id);
                UserDto.IssueCreated = CountIssueCreated;
                int CountIssueAssign = _assignRepository.Count(x => x.UserID == result.user.Id);
                UserDto.IssueAssign = CountIssueAssign;
                int CountIssueCreatedFinish = _issueRepository.Count(x => x.CreatorId == result.user.Id && x.FinishDate != null);
                int CountIssueAssignFinish = (from issue in _issueRepository
                                              join assignee in _assignRepository on issue.Id equals assignee.IssueID
                                              join user in _userRepository on assignee.UserID equals user.Id
                                              where issue.FinishDate != null && assignee.UserID != issue.CreatorId && assignee.UserID == result.user.Id
                                              select new {issue}).Count();

                int issueOutOfDateCreated = _issueRepository.Count(x => x.CreatorId == result.user.Id && x.DueDate <= DateTime.Today);
                int issueOutOfDateAssign = (from issue in _issueRepository
                                            join assignee in _assignRepository on issue.Id equals assignee.IssueID
                                            join user in _userRepository on assignee.UserID equals user.Id
                                            where issue.DueDate <= DateTime.Today && assignee.UserID != issue.CreatorId && assignee.UserID == result.user.Id
                                            select new { issue }).Count();

                int CountProjectCreated = _projectRepository.Count(x => x.CreatorId == result.user.Id);
                UserDto.ProjectCreated = CountProjectCreated;
                decimal couIssueAssign1 = (from issue in _issueRepository
                                           join assignee in _assignRepository on issue.Id equals assignee.IssueID
                                           join user in _userRepository on assignee.UserID equals user.Id
                                           where assignee.UserID != issue.CreatorId && assignee.UserID == result.user.Id
                                           select new { issue }).Count();

                decimal CountIssueFinish;
                decimal CountIssueOutOfDate;
                if ((CountIssueCreated + couIssueAssign1) == 0)
                {
                    CountIssueFinish = 0;
                    CountIssueOutOfDate = 0;
                }
                else
                {
                    CountIssueFinish = ((CountIssueCreatedFinish + CountIssueAssignFinish) / (CountIssueCreated + couIssueAssign1)) * 100;
                    CountIssueOutOfDate = ((issueOutOfDateAssign + issueOutOfDateCreated) / (CountIssueCreated + couIssueAssign1)) * 100;
                }

                UserDto.IssueFinish = (long)CountIssueFinish;
                UserDto.CountIssueFinish = (CountIssueCreatedFinish + CountIssueAssignFinish);
                UserDto.CountIssue = (int)(CountIssueCreated + couIssueAssign1);
                UserDto.IssueOutOfDate = (int)CountIssueOutOfDate;
                UserDto.CountIssueOutOfDate = (issueOutOfDateAssign + issueOutOfDateCreated);
                return UserDto;
            }).ToList();
            return UserDtos;

        }
        public async Task DeleteUserById(string Id)
        {
            try
            {
                var user = await _userRepository.GetAsync(Id);

                if (user == null)

                {
                    throw new EntityNotFoundException(typeof(Member), Id);
                }
                else
                {
                    var project = _projectRepository.Where(x => x.CreatorId == Id).ToArray();
                    var member = _memberRepository.Where(x => x.UserID == Id).ToArray();
                    var status = _statusRepository.Where(x => x.CreatorId == Id).ToArray();
                    var category = _categoryRepository.Where(x => x.CreatorId == Id).ToArray();
                    for (int i = 0; i < member.Length; i++)
                    {
                        var issues = await _issueRepository.
                        GetListAsync(x => x.CreatorId == member[i].UserID && x.ProjectID == member[i].ProjectID);
                        var assignees = await _assignRepository.GetListAsync(x => x.UserID == member[i].UserID);
                        if (assignees.Any())
                        {
                            await _assignRepository.DeleteManyAsync(assignees);
                        }

                        if (issues.Any())
                        {
                            for (int j = 0; j < issues.Count; j++)
                            {
                                var assigneesIssue = await _assignRepository.GetListAsync(x => x.IssueID == issues[j].Id);
                                if (assigneesIssue.Any())
                                {
                                    await _assignRepository.DeleteManyAsync(assigneesIssue);
                                }
                                await _issueRepository.DeleteAsync(issues[j].Id);
                            }
                        }
                        await _memberRepository.DeleteAsync(member[i]);
                    }
                    if (project.Any())
                    {
                        await _projectRepository.DeleteManyAsync(project);
                    }
                    if (status.Any())
                    {
                        await _statusRepository.DeleteManyAsync(status);
                    }
                    if (category.Any())
                    {
                        await _categoryRepository.DeleteManyAsync(category);
                    }

                    await _userRepository.DeleteAsync(Id);

                }
            }
            catch
            {
                throw new UserFriendlyException("An error throw when delete!");
            }
        }
        public async Task<List<ProjectDto>> GetListProject(String input, int dueDate, string userid)
        {
            if (input.IsNullOrWhiteSpace() || input == "null")
            {
                input = "";
            }
            if (userid.IsNullOrWhiteSpace() || userid == "null")
            {
                userid = "";
            }

            var query = from project in _projectRepository
                        join user in _userRepository on project.CreatorId equals user.Id
                        where project.Name.Contains(input) && project.CreatorId.Contains(userid)
                        select new { project, user };
            var queryResult = await AsyncExecuter.ToListAsync(query);

            var ProjectDtos = queryResult.Select(result =>
            {
                var ProjectDto = ObjectMapper.Map<Project, ProjectDto>(result.project);
                int countIssue = _issueRepository.Count(x => x.ProjectID == result.project.Id);
                decimal countIssueClosed = _issueRepository.Count(x => x.ProjectID == result.project.Id && x.FinishDate != null);
                int countMember = _memberRepository.Count(x => x.ProjectID == result.project.Id);

                if (dueDate != 0)
                {
                    decimal countIssueDueDateXDay = _issueRepository.Count(x => x.DueDate <= DateTime.Today.AddDays(dueDate)
                     && x.DueDate >= DateTime.Today && x.ProjectID == result.project.Id);
                    if (countIssue == 0)
                    {
                        ProjectDto.countIssueDueDateATime = 0;
                        ProjectDto.issueDueDateATime = 0;
                    }
                    else
                    {
                        ProjectDto.countIssueDueDateATime = (int)((countIssueDueDateXDay / countIssue) * 100);
                        ProjectDto.issueDueDateATime = (int)countIssueDueDateXDay;
                    }

                }
                decimal issueDueDate = _issueRepository.Count(x => x.DueDate <= DateTime.Today &&
                  x.ProjectID == result.project.Id);

                if (countIssue == 0)
                {
                    ProjectDto.countIssueDueDate = 0;
                    ProjectDto.countIssueClose = 0;
                }
                else
                {
                    ProjectDto.countIssueDueDate = (int)((issueDueDate / countIssue) * 100);
                    ProjectDto.countIssueClose = (int)((countIssueClosed / countIssue) * 100);
                }
                ProjectDto.UserId = result.user.Id;
                ProjectDto.userName = result.user.Name;
                ProjectDto.CountMember = countMember;
                ProjectDto.issueClose = (int)countIssueClosed;
                ProjectDto.issueDueDate = (int)issueDueDate;
                ProjectDto.countIssue = countIssue;
                return ProjectDto;
            }).ToList();
            return ProjectDtos;
        }
        public IEnumerable<ReturnUpdateDto> ChartIssueOfProject(string IdProject)
        {

            if (IdProject.IsNullOrWhiteSpace() || IdProject == "")
            {
                var timeissue = (from issue in _issueRepository
                                 orderby issue.CreationTime ascending
                                 select new TimeIssueDTO
                                 {
                                     CreationTime = issue.CreationTime.ToString("dd/MM/yyyy"),
                                     //FinishDate = issue.FinishDate.GetValueOrDefault().ToString("dd/MM/yyyy")
                                 }
                      ).ToList();
                var newArray = new List<string>();
                if (timeissue.Count() == 0)
                {
                    IEnumerable<ReturnUpdateDto> x = Enumerable.Empty<ReturnUpdateDto>();
                    return x;
                }
                timeissue.ForEach(e =>
                {
                    newArray.Add(e.CreationTime);
                    //newArray.Add(e?.FinishDate);
                });
                var result = newArray.Distinct().ToList();
                if (newArray.IndexOf("01/01/0001") != -1)
                {
                    result.RemoveAt(newArray.IndexOf("01/01/0001"));
                }
                result.OrderBy(x => x.Replace("/", "")).ToList();
                var DateGroup = result.GroupBy(x => x.Substring(06));

                var query = _issueRepository.ToList();
                //var query = (from issue1 in _issueRepository
                //             orderby issue1.CreationTime ascending
                //             select new { issue1 }
                //      ).ToList();

                var returnData = DateGroup.Select(y =>
                {
                    var data = new ReturnUpdateDto
                    {
                        Year = y.Key,
                        CountCreate = query.Where(x => x.CreationTime.ToString("yyyy") == y.Key).Count(),
                        CountFinish = query.Where(x => x.FinishDate.GetValueOrDefault().ToString("yyyy") == y.Key).Count(),
                        Data = y.GroupBy(t => t.Substring(3, 7), (key, t) => new
                        {
                            Month = key,
                            CountCreate = query.Where(x => x.CreationTime.ToString("MM/yyyy") == key).Count(),
                            CountFinish = query.Where(x => x.FinishDate.GetValueOrDefault().ToString("MM/yyyy") == key).Count(),

                        })
                    };
                    return data;
                });
                return returnData;
            }
            else
            {
                var timeissue = (from issue in _issueRepository
                                 where issue.ProjectID.ToString() == IdProject
                                 orderby issue.CreationTime ascending
                                 select new TimeIssueDTO
                                 {
                                     CreationTime = issue.CreationTime.ToString("dd/MM/yyyy"),
                                     // FinishDate = issue.FinishDate.GetValueOrDefault().ToString("dd/MM/yyyy")
                                 }
                        ).ToList();
                var newArray = new List<string>();
                if (timeissue.Count() == 0)
                {
                    IEnumerable<ReturnUpdateDto> x = Enumerable.Empty<ReturnUpdateDto>();
                    return x;
                }
                timeissue.ForEach(e =>
                {
                    newArray.Add(e.CreationTime);
                    // newArray.Add(e?.FinishDate);
                });
                var result = newArray.Distinct().ToList();
                if (newArray.IndexOf("01/01/0001") != -1)
                {
                    result.RemoveAt(newArray.IndexOf("01/01/0001"));
                }
                result.OrderBy(x => x.Replace("/", "")).ToList();
                var DateGroup = result.GroupBy(x => x.Substring(06));

                var query = _issueRepository.Where(x => x.ProjectID.ToString() == IdProject).ToList();

                var returnData = DateGroup.Select(y =>
                {
                    var data = new ReturnUpdateDto
                    {
                        Year = y.Key,
                        CountCreate = query.Where(x => x.CreationTime.ToString("yyyy") == y.Key).Count(),
                        CountFinish = query.Where(x => x.FinishDate.GetValueOrDefault().ToString("yyyy") == y.Key).Count(),
                        Data = y.GroupBy(t => t.Substring(3, 7), (key, t) => new
                        {
                            Month = key,
                            CountCreate = query.Where(x => x.CreationTime.ToString("MM/yyyy") == key).Count(),
                            CountFinish = query.Where(x => x.FinishDate.GetValueOrDefault().ToString("MM/yyyy") == key).Count(),

                        })
                    };
                    return data;
                });
                return returnData;
            }


        }
        public async Task<string> getUserName(string userId)
        {
            var user = await _userRepository.GetAsync(userId);
            return user.Name;
        }
        public async Task<List<StatusDTO>> GetListStatusAsync(string IdProject)
        {

            if (IdProject.IsNullOrWhiteSpace() || IdProject == "")
            {
                var query = from status in _statusRepository
                            orderby status.CurrentIndex ascending
                            select new
                            { status };

                var queryResult = await AsyncExecuter.ToListAsync(query);

                var statusDtos = queryResult.Select(result =>
                {
                    var statusDto = ObjectMapper.Map<Status, StatusDTO>(result.status);
                    statusDto.IssueList = new List<IssuesDto> { };
                    statusDto.Name = result.status.Name;
                    statusDto.Id = result.status.Id;
                    decimal issueStatus = _issueRepository.Count(x => x.StatusID == result.status.Id);
                    statusDto.CountIssue = _issueRepository.Count(x => x.StatusID == result.status.Id);
                    decimal countIssueAll = _issueRepository.Count();
                    if (countIssueAll == 0)
                    {
                        statusDto.CountIssuePercent = 0;
                    }
                    else
                    {
                        statusDto.CountIssuePercent = (int)((issueStatus / countIssueAll) * 100);
                    }
                    return statusDto;
                }).ToList();

                return statusDtos;
            }
            else
            {
                var query = from status in _statusRepository
                            orderby status.CurrentIndex ascending
                            select new
                            { status };

                var queryResult = await AsyncExecuter.ToListAsync(query);

                var statusDtos = queryResult.Select(result =>
                {
                    var statusDto = ObjectMapper.Map<Status, StatusDTO>(result.status);
                    statusDto.IssueList = new List<IssuesDto> { };
                    statusDto.Name = result.status.Name;
                    statusDto.Id = result.status.Id;
                    decimal issueStatus = _issueRepository.Count(x => x.ProjectID.ToString() == IdProject && x.StatusID == result.status.Id);
                    statusDto.CountIssue = _issueRepository.Count(x => x.ProjectID.ToString() == IdProject && x.StatusID == result.status.Id);
                    decimal countIssueAll = _issueRepository.Count(x => x.ProjectID.ToString() == IdProject);
                    if (countIssueAll == 0)
                    {
                        statusDto.CountIssuePercent = 0;
                    }
                    else
                    {
                        statusDto.CountIssuePercent = (int)((issueStatus / countIssueAll) * 100);
                    }
                    return statusDto;
                }).ToList();

                return statusDtos;
            }

        }
        public async Task<List<IssuesDto>> GetListIssueAll(String input, string IdProject, string IdStatus, string IdCate, int dueDate, string IdUser, string idUserAssign)
        {
            if (input.IsNullOrWhiteSpace() || input == "null")
            {
                input = "";
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
            if (idUserAssign.IsNullOrWhiteSpace() || idUserAssign == "null")
            {
                idUserAssign = "";
            }
            if (dueDate == 0 || dueDate.ToString() == null)
            {
                if (idUserAssign == null || idUserAssign == "")
                {

                    var query = from issue in _issueRepository
                                join status in _statusRepository on issue.StatusID equals status.Id
                                join cat in _categoryRepository on issue.CategoryID equals cat.Id into termCategory
                                from c in termCategory.DefaultIfEmpty()
                                join project in _projectRepository on issue.ProjectID equals project.Id
                                join user in _userRepository on issue.CreatorId equals user.Id into termUser
                                from u in termUser.DefaultIfEmpty()
                                where issue.Name.Contains(input) && issue.CreatorId.Contains(IdUser) && issue.ProjectID.ToString().Contains(IdProject)
                                 && issue.StatusID.ToString().Contains(IdStatus)
                                && issue.CategoryID.ToString().Contains(IdCate)
                                orderby issue.CreationTime ascending
                                select new
                                {
                                    issue,
                                    catName = c == null ? null : c.Name,
                                    userName = u == null ? null : u.Name
                                };
                    var queryResult = await AsyncExecuter.ToListAsync(query);

                    var IssueDtos = queryResult.Select(result =>
                    {
                        var IssueDto = ObjectMapper.Map<Issue, IssuesDto>(result.issue);
                        IssueDto.Name = result.issue.Name;
                        IssueDto.UserName = result.userName;
                        var p = _projectRepository.GetAsync(result.issue.ProjectID);
                        var project = ObjectMapper.Map<Project, ProjectDto>(p.Result);
                        IssueDto.ProjectName = project.Name;
                        var s = _statusRepository.GetAsync(result.issue.StatusID);
                        var status = ObjectMapper.Map<Status, StatusDTO>(s.Result);
                        IssueDto.StatusName = status.Name;
                        IssueDto.CategoryName = result.catName;
                        decimal issueDueDate = _issueRepository.Where(x => x.DueDate <= DateTime.Today).Count();
                        IssueDto.CountIssueDueDate = (int)issueDueDate;
                        IssueDto.StartDate = result.issue.CreationTime;
                        IssueDto.DueDate = result.issue.DueDate;
                        IssueDto.NzColor = status.NzColor;
                        return IssueDto;
                    }).ToList();
                    return IssueDtos;
                }
                else
                {
                    var query = from issue in _issueRepository
                                join status in _statusRepository on issue.StatusID equals status.Id
                                join assign in _assignRepository on issue.Id equals assign.IssueID into termAssignee
                                from a in termAssignee.DefaultIfEmpty()
                                join cat in _categoryRepository on issue.CategoryID equals cat.Id into termCategory
                                from c in termCategory.DefaultIfEmpty()
                                join user in _userRepository on issue.CreatorId equals user.Id into termUser
                                from u in termUser.DefaultIfEmpty()
                                where issue.Name.Contains(input) && issue.CreatorId.Contains(IdUser) && issue.ProjectID.ToString().Contains(IdProject)
                                 && issue.StatusID.ToString().Contains(IdStatus)
                                && issue.CategoryID.ToString().Contains(IdCate)
                                && (a.UserID == idUserAssign)
                                orderby issue.CreationTime ascending
                                select new
                                {
                                    issue,
                                    catName = c == null ? null : c.Name,
                                    userName = u == null ? null : u.Name
                                };
                    var queryResult = await AsyncExecuter.ToListAsync(query);

                    var IssueDtos = queryResult.Select(result =>
                    {
                        var IssueDto = ObjectMapper.Map<Issue, IssuesDto>(result.issue);
                        IssueDto.Name = result.issue.Name;
                        IssueDto.UserName = result.userName;
                        var p = _projectRepository.GetAsync(result.issue.ProjectID);
                        var project = ObjectMapper.Map<Project, ProjectDto>(p.Result);
                        IssueDto.ProjectName = project.Name;
                        var s = _statusRepository.GetAsync(result.issue.StatusID);
                        var status = ObjectMapper.Map<Status, StatusDTO>(s.Result);
                        IssueDto.StatusName = status.Name;
                        IssueDto.CategoryName = result.catName;
                        decimal issueDueDate = _issueRepository.Where(x => x.DueDate <= DateTime.Today).Count();
                        IssueDto.CountIssueDueDate = (int)issueDueDate;
                        IssueDto.StartDate = result.issue.CreationTime;
                        IssueDto.DueDate = result.issue.DueDate;
                        IssueDto.NzColor = status.NzColor;
                        return IssueDto;
                    }).ToList();
                    return IssueDtos;
                }

            }
            else
            {
                if (idUserAssign == null || idUserAssign == "")
                {
                    var query = from issue in _issueRepository
                                join status in _statusRepository on issue.StatusID equals status.Id
                                join cat in _categoryRepository on issue.CategoryID equals cat.Id into termCategory
                                from c in termCategory.DefaultIfEmpty()
                                join user in _userRepository on issue.CreatorId equals user.Id into termUser
                                from u in termUser.DefaultIfEmpty()
                                where issue.Name.Contains(input) && issue.CreatorId.Contains(IdUser) && issue.ProjectID.ToString().Contains(IdProject)
                                && (issue.DueDate <= DateTime.Today.AddDays(dueDate))
                                  && issue.StatusID.ToString().Contains(IdStatus)
                                && issue.CategoryID.ToString().Contains(IdCate)
                                orderby issue.CreationTime ascending
                                select new
                                {
                                    issue,
                                    catName = c == null ? null : c.Name,
                                    userName = u == null ? null : u.Name
                                };
                    var queryResult = await AsyncExecuter.ToListAsync(query);

                    var IssueDtos = queryResult.Select(result =>
                    {
                        var IssueDto = ObjectMapper.Map<Issue, IssuesDto>(result.issue);
                        IssueDto.Name = result.issue.Name;
                        IssueDto.UserName = result.userName;
                        var p = _projectRepository.GetAsync(result.issue.ProjectID);
                        var project = ObjectMapper.Map<Project, ProjectDto>(p.Result);
                        IssueDto.ProjectName = project.Name;
                        var s = _statusRepository.GetAsync(result.issue.StatusID);
                        var status = ObjectMapper.Map<Status, StatusDTO>(s.Result);
                        IssueDto.StatusName = status.Name;
                        IssueDto.CategoryName = result.catName;
                        decimal issueDueDate = _issueRepository.Where(x => x.DueDate <= DateTime.Today).Count();
                        IssueDto.CountIssueDueDate = (int)issueDueDate;
                        IssueDto.StartDate = result.issue.CreationTime;
                        IssueDto.DueDate = result.issue.DueDate;
                        IssueDto.NzColor = status.NzColor;
                        return IssueDto;
                    }).ToList();
                    return IssueDtos;
                }
                else
                {
                    var query = from issue in _issueRepository
                                join status in _statusRepository on issue.StatusID equals status.Id
                                join assign in _assignRepository on issue.Id equals assign.IssueID into termAssignee
                                from a in termAssignee.DefaultIfEmpty()
                                join cat in _categoryRepository on issue.CategoryID equals cat.Id into termCategory
                                from c in termCategory.DefaultIfEmpty()
                                join user in _userRepository on issue.CreatorId equals user.Id into termUser
                                from u in termUser.DefaultIfEmpty()
                                where issue.Name.Contains(input) && issue.CreatorId.Contains(IdUser) && issue.ProjectID.ToString().Contains(IdProject)
                                && (issue.DueDate <= DateTime.Today.AddDays(dueDate))
                                  && issue.StatusID.ToString().Contains(IdStatus)
                                && issue.CategoryID.ToString().Contains(IdCate)
                                && (a.UserID == idUserAssign)
                                orderby issue.CreationTime ascending
                                select new
                                {
                                    issue,
                                    catName = c == null ? null : c.Name,
                                    userName = u == null ? null : u.Name
                                };
                    var queryResult = await AsyncExecuter.ToListAsync(query);

                    var IssueDtos = queryResult.Select(result =>
                    {
                        var IssueDto = ObjectMapper.Map<Issue, IssuesDto>(result.issue);
                        IssueDto.Name = result.issue.Name;
                        IssueDto.UserName = result.userName;
                        var p = _projectRepository.GetAsync(result.issue.ProjectID);
                        var project = ObjectMapper.Map<Project, ProjectDto>(p.Result);
                        IssueDto.ProjectName = project.Name;
                        var s = _statusRepository.GetAsync(result.issue.StatusID);
                        var status = ObjectMapper.Map<Status, StatusDTO>(s.Result);
                        IssueDto.StatusName = status.Name;
                        IssueDto.CategoryName = result.catName;
                        decimal issueDueDate = _issueRepository.Where(x => x.DueDate <= DateTime.Today).Count();
                        IssueDto.CountIssueDueDate = (int)issueDueDate;
                        IssueDto.StartDate = result.issue.CreationTime;
                        IssueDto.DueDate = result.issue.DueDate;
                        IssueDto.NzColor = status.NzColor;
                        return IssueDto;
                    }).ToList();
                    return IssueDtos;
                }
            }

        }
    }

}
