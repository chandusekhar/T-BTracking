using BugTracking.Assignees;
using BugTracking.Azure;
using BugTracking.Azures;
using BugTracking.Categories;
using BugTracking.Comments;
using BugTracking.ConditionTypeWit;
using BugTracking.Conversation;
using BugTracking.Issues;
using BugTracking.Members;
using BugTracking.Statuss;
using BugTracking.Users;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;
using static BugTracking.Azure.AzureAppService;

namespace BugTracking.Projects
{
    [RemoteService(IsEnabled = false)]
    [Authorize("BugTracking.Users")]
    public class ProjectAppService : BugTrackingAppService, IProjectAppService, ITransientDependency
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ProjectManager _projectManager;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly IMemberRepository _memberRepository;
        private readonly IUserRepository _userRepository;
        private readonly MemberManager _memberManager;
        private readonly IIssueRepository _issueRepository;
        private readonly IAssigneeRepository _assigneeRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly UserManager _userManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IAzureRepository _azureRepository;
        private readonly AzureAppService _azureAppService;
        private readonly IConversationRepository _conversationRepostiry;
        public ProjectAppService(
            UserManager userManager,
            ITMTCurrentUser tmtCurrentUser,
            ProjectManager projectManager,
            IProjectRepository projectRepository,
            IMemberRepository memberRepository,
            MemberManager memberManager,
            IUserRepository userRepository,
            IIssueRepository issueRepository,
            IAssigneeRepository assigneeRepository,
        IStatusRepository statusRepository,
            ICategoryRepository categoryRepository,
            ICommentRepository commentRepository,
            IBackgroundJobClient backgroundJobClient,
            IAzureRepository azureRepository,
            AzureAppService azureAppService,
            IConversationRepository conversationRepository
        )
        {
            _memberRepository = memberRepository;
            _tmtCurrentUser = tmtCurrentUser;
            _projectRepository = projectRepository;
            _projectManager = projectManager;
            _memberManager = memberManager;
            _userRepository = userRepository;
            _issueRepository = issueRepository;
            _assigneeRepository = assigneeRepository;
            _statusRepository = statusRepository;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            _commentRepository = commentRepository;
            _backgroundJobClient = backgroundJobClient;
            _azureRepository = azureRepository;
            _azureAppService = azureAppService;
            _conversationRepostiry = conversationRepository;
        }

        public async Task<ProjectDto> CreateAsync(CreateProjectDto input)
        {
            var userId = _tmtCurrentUser.Id;
            if (_projectRepository.Any(x => x.Name.ToLower().Trim() == input.Name.ToLower().Trim()))
            {
                throw new UserFriendlyException("Project Name Exist! Try Another Name!");
            }
            if (input.Name.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Input name do not null! Try Another Name!");
            }
            var project = await _projectManager.CreateAsync(input.Name.Trim(), "#DAA520", Guid.Empty, "Task");

            await _projectRepository.InsertAsync(project);
            var member = await _memberManager.CreateAsync(
                    project.Id,
                    userId
                );
            await _memberRepository.InsertAsync(member);

            return ObjectMapper.Map<Project, ProjectDto>(project);
        }
        public List<string> GetCheckName(string name)
        {
            var rs = new List<string> { };
            if (_projectRepository.Any(x => x.Name == name))
            {
                rs.Add("Project Name exist! Try another name!");
            }
            return rs;
        }
        public async Task DeleteAsync(Guid id)
        {
            var proId = id.ToString();
            var CurrentUserId = _tmtCurrentUser.GetId();
            var project = await _projectRepository.GetAsync(id);
            var conver = _conversationRepostiry.Where(x => x.idProject == proId).FirstOrDefault();
            if (project.ProjectIdTFS != Guid.Empty)
            {
                if (_azureRepository.Any())
                {
                    _backgroundJobClient.Enqueue<IAzureAppService>(x => x.DeleteProject(project.Id, CurrentUserId));
                }
            }
            var issues = _issueRepository.Where(x => x.ProjectID == id).ToList();
            if (issues != null)
            {
                for (int i = 0; i < issues.Count(); i++)
                {
                    var assigns = _assigneeRepository.Where(x => x.IssueID == issues[i].Id);
                    if (assigns != null)
                    {
                        foreach (Assignee assign in assigns)
                        {
                            await _assigneeRepository.DeleteAsync(assign);
                        }
                    }
                    await _issueRepository.DeleteAsync(issues[i].Id);
                }
            }
            if (conver != null)
            {
                await _conversationRepostiry.DeleteAsync(conver);

            }
            await _projectRepository.DeleteAsync(id);
        }

        public async Task<ProjectDto> GetAsyncByID(Guid id)
        {
            var Today = DateTime.UtcNow;
            var project = await _projectRepository.GetAsync(id);
            var listUser = await _userRepository.GetListAsync();
            var issueOverDueDate = from projectJ in _projectRepository
                                   join issue in _issueRepository on projectJ.Id equals issue.ProjectID
                                   join status in _statusRepository on issue.StatusID equals status.Id
                                   where issue.DueDate <= Today && status.Name != "Closed"
                                   select new { issue };
            int countMember = 0;
            var projectDto = ObjectMapper.Map<Project, ProjectDto>(project);
            projectDto.MemberList = new List<MemberDto> { };
            foreach (Member member in _memberRepository
            .Where(member => member.ProjectID == id)
            )
            {
                var memberDto = ObjectMapper.Map<Member, MemberDto>(member);
                var user = _userRepository.Where(u => u.Id == memberDto.UserID).FirstOrDefault();
                memberDto.UserID = user.Id;
                memberDto.UserName = user.Name;
                projectDto.MemberList.Add(memberDto);
                countMember++;
            }
            for (int i = 0; i < listUser.Count(); i++)
            {
                if (_userManager.CheckRoleByUserId(listUser[i].Id, "admin").Result) projectDto.IdUserAdmin = listUser[i].Id;
            }
            projectDto.CountMember = projectDto.MemberList.Count;
            projectDto.UserId = project.CreatorId;
            projectDto.countIssueDueDateATime = issueOverDueDate.Count();
            return projectDto;
        }

        public async Task<ListResultDto<ProjectDto>> GetListAsync()
        {
            var projects = await _projectRepository.GetListAsync();

            return new ListResultDto<ProjectDto>(
               ObjectMapper.Map<List<Project>, List<ProjectDto>>(projects)
           );
        }

        public async Task UpdateAsync(Guid id, UpdateProjectDto input)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var project = await _projectRepository.GetAsync(id);
            if (project.ProjectIdTFS != Guid.Empty)
            {
                await _azureAppService.UpdateProject(project.ProjectIdTFS, input.Name, CurrentUserId, input.Description);
            }
            project.Name = input.Name.Trim();
            if (input.WitType == "Bug") project.NzColor = "#F50";
            if (input.WitType == "Epic") project.NzColor = "#F50";
            if (input.WitType == "Feature") project.NzColor = "#9400D3";
            if (input.WitType == "Issue") project.NzColor = "#483D8B";
            if (input.WitType == "Task") project.NzColor = "#DAA520";
            if (input.WitType == "Test Case") project.NzColor = "#808080";
            if (input.WitType == "User Story") project.NzColor = "#0198c7";
            project.WitType = input.WitType;
            await _projectRepository.UpdateAsync(project);
        }

        public async Task<ListResultDto<ProjectDto>> GetListAsyncByUserId()
        {
            var userId = _tmtCurrentUser.Id;
            IQueryable<Project> projectQueryable = await _projectRepository.GetQueryableAsync();
            IQueryable<Member> memberQueryable = await _memberRepository.GetQueryableAsync();
            //Create a query
            var query = from project in projectQueryable
                        join member in memberQueryable on project.Id equals member.ProjectID
                        where member.UserID == userId
                        select project;

            //Execute the query to get list of people
            var projects = query.ToList();

            //Convert to DTO and return to the client
            return new ListResultDto<ProjectDto>(
                ObjectMapper.Map<List<Project>, List<ProjectDto>>(projects));
        }

        public async Task<List<ProjectDto>> GetListProjectByUserId(string userId, string filter)
        {
            if (userId.IsNullOrWhiteSpace() || userId == "null")
            {
                userId = "";
            }
            if (filter.IsNullOrWhiteSpace() || filter == "null")
            {
                filter = "";
            }

            //if( await _userManager.CheckRoleByUserId(userId,"admin")) {
            //    var query = from project in projectQueryable
            //                where project.Name.Contains(filter)
            //                orderby project.CreationTime descending
            //                select new
            //                { project };
            //    var queryResult = await AsyncExecuter.ToListAsync(query);

            //    var ProjectDtos = queryResult.Select(result =>
            //    {
            //        var ProjectDto = ObjectMapper.Map<Project, ProjectDto>(result.project);
            //        ProjectDto.MemberList = new List<MemberDto> { };
            //        int countMember = 0;
            //        int coutAdd = 0;
            //        foreach (Member member in _memberRepository
            //        .Where(member => member.ProjectID == result.project.Id)
            //        )
            //        {
            //            var memberDto = ObjectMapper.Map<Member, MemberDto>(member);
            //            var user = _userRepository.FirstOrDefault(u => u.Id == member.UserID);
            //            memberDto.UserID = user.Id;
            //            memberDto.UserName = user.Name;
            //            if (countMember <= 9)
            //            {
            //                ProjectDto.MemberList.Add(memberDto);
            //            }
            //            if (countMember >= 10)
            //            {
            //                coutAdd++;
            //            }
            //            countMember++;
            //        }
            //        ProjectDto.CountAdd = coutAdd;
            //        ProjectDto.CountMember = countMember;
            //        ProjectDto.countIssue = _issueRepository.Count(x => x.ProjectID == result.project.Id);

            //        return ProjectDto;
            //    }).ToList();
            //    return ProjectDtos;
            //}
            //else
            //{
            var query = from project in _projectRepository
                        join member in _memberRepository on project.Id equals member.ProjectID
                        join user in _userRepository on member.UserID equals user.Id
                        where member.UserID == userId && project.Name.Contains(filter)
                        orderby project.CreationTime descending
                        select new
                        {
                            project
                        };
            var queryResult = await AsyncExecuter.ToListAsync(query);

            var ProjectDtos = queryResult.Select(result =>
            {
                var ProjectDto = ObjectMapper.Map<Project, ProjectDto>(result.project);
                ProjectDto.MemberList = new List<MemberDto> { };
                int countMember = 0;
                int countAdd = 0;
                foreach (Member member in _memberRepository
                .Where(member => member.ProjectID == result.project.Id)
                )
                {
                    var memberDto = ObjectMapper.Map<Member, MemberDto>(member);
                    var user = _userRepository.FirstOrDefault(u => u.Id == memberDto.UserID);
                    memberDto.UserID = user.Id;
                    memberDto.UserName = user.Name;
                    if (countMember <= 9)
                    {
                        ProjectDto.MemberList.Add(memberDto);
                    }
                    if (countMember >= 10)
                    {
                        countAdd++;
                    }
                    countMember++;
                }
                ProjectDto.CountAdd = countAdd;
                ProjectDto.CountMember = countMember;
                ProjectDto.countIssue = _issueRepository.Count(x => x.ProjectID == result.project.Id);
                ProjectDto.userName = _userRepository.GetAsync(x => x.Id == result.project.CreatorId).Result.Name;
                ProjectDto.IsSynced = result.project.ProjectIdTFS != Guid.Empty;

                return ProjectDto;
            }).ToList();
            return ProjectDtos;
            //   } 
        }
        public ItemProjectDTO GetItemProject(Guid idProject)
        {
            double issue = _issueRepository.Where(x => x.ProjectID == idProject).Count();
            var member = _memberRepository.Where(x => x.ProjectID == idProject).Count();
            var date = DateTime.Now.Subtract(TimeSpan.FromDays(7));

            double issueInAWeek = _issueRepository.Where(x => x.FinishDate > date && x.ProjectID == idProject).Count();
            double issueAdd = _issueRepository.Where(x => x.CreationTime > date && x.ProjectID == idProject).Count();
            double percent = ((issueInAWeek / (issue == 0 ? 1 : issue)) * 100);
            double percentAdd = ((issueAdd / (issue == 0 ? 1 : issue)) * 100);
            var query = from iss in _issueRepository
                        join status in _statusRepository on iss.StatusID equals status.Id
                        where status.Name != "Closed" && iss.ProjectID == idProject
                        select new { };

            var notClosed = query.Count();
            return new ItemProjectDTO()
            {
                AssigneeCount = member,
                issueCount = issue,
                percentIssue = percent,
                percentAdd = percentAdd,
                notClosed = notClosed,
            };
        }



        public IEnumerable<ReturnUpdateDto> ChartIssue(Guid IdProject)
        {
            var timeissue = (from issue in _issueRepository
                             where issue.ProjectID == IdProject
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
            var query = _issueRepository.Where(x => x.ProjectID == IdProject).ToList();
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
                        Data = t.GroupBy(e => e.Substring(0), (key, e) => new
                        {
                            Date = key,
                            CountCreate = query.Where(e => e.CreationTime.ToString("dd/MM/yyyy") == key).Count(),
                            CountFinish = query.Where(e => e.FinishDate.GetValueOrDefault().ToString("dd/MM/yyyy") == key).Count(),
                        })
                    })
                };
                return data;
            });
            return returnData;
        }

        public Dictionary<string, List<ExportFileDTO>> ExportFile()
        {
            var query = from issue in _issueRepository
                        join user in _userRepository on issue.CreatorId equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join status in _statusRepository on issue.StatusID equals status.Id
                        join project in _projectRepository on issue.ProjectID equals project.Id
                        join cat in _categoryRepository on issue.CategoryID equals cat.Id into termCategory
                        from c in termCategory.DefaultIfEmpty()
                        orderby issue.IssueLevel descending
                        select new ExportFileDTO()
                        {
                            IssueName = issue.Name,
                            Description = issue.Description,
                            UserName = u == null ? null : u.Name,
                            StatusName = status == null ? null : status.Name,
                            ProjectName = project == null ? null : project.Name,
                            CategoryName = c == null ? null : c.Name,
                            DueDate = issue.DueDate,
                            FinishDate = issue.FinishDate,
                            StartDate = issue.StartDate,

                        };
            var queryResult = query.ToList();
            var newGroup = queryResult.GroupBy(x => x.ProjectName);

            Dictionary<string, List<ExportFileDTO>> listIssue = new Dictionary<string, List<ExportFileDTO>>();
            foreach (var group in newGroup)
            {
                listIssue.Add(group.Key, group.ToList());

            }
            return listIssue;
        }
        public async Task<CategoryStatisticDto> GetCategoryStatisticAsync(Guid id)
        {
            List<int> issues = new List<int>();
            List<string> categoryList = new List<string>();
            var query = from project in _projectRepository
                        join issue in _issueRepository on project.Id equals issue.ProjectID
                        join category in _categoryRepository on issue.CategoryID equals category.Id
                        where project.Id == id
                        select new { category };
            var queryProjectResult = await AsyncExecuter.ToListAsync(query.Distinct());
            for (var i = 0; i < queryProjectResult.Count(); i++)
            {
                categoryList.Add(queryProjectResult[i].category.Name);
                issues.Add((from project in _projectRepository
                            join issue in _issueRepository on project.Id equals issue.ProjectID
                            join category in _categoryRepository on issue.CategoryID equals category.Id
                            where project.Id == id && issue.CategoryID == queryProjectResult[i].category.Id
                            select new { issue }).Count());
            }
            return new CategoryStatisticDto
            {
                Categories = categoryList.ToArray(),
                Issues = issues.ToArray()
            };
        }

        public async Task<Project1StatisticDto> ProjectStatisticAsync(Guid Id, int lastDay)
        {
            var Now = DateTime.UtcNow;
            int Created = 0;
            int Closed = 0;
            decimal pCreated = 0;
            decimal pClosed = 0;
            var totalIssue = _issueRepository.Count(x => x.ProjectID == Id);
            if (totalIssue != 0)
            {
                Created = _issueRepository.Count(x => x.ProjectID == Id && x.CreationTime.AddDays(lastDay).Date >= Now.Date);
                Closed = (from project in _projectRepository
                          join issue in _issueRepository on project.Id equals issue.ProjectID
                          join status in _statusRepository on issue.StatusID equals status.Id
                          where project.Id == Id && status.Name == "Closed" && issue.FinishDate.Value.AddDays(lastDay).Date >= Now.Date
                          select new { issue }).Count();
                pCreated = (Math.Round(Created / (decimal)totalIssue, 2)) * 100;
                pClosed = (Math.Round(Closed / (decimal)totalIssue, 2)) * 100;
            }

            List<Member1StatisticDto> member1StatisticDto = new List<Member1StatisticDto>();

            var members = await _memberRepository.GetListAsync(x => x.ProjectID == Id);

            foreach (Member member in members)
            {
                var memberMap = ObjectMapper.Map<Member, Member1StatisticDto>(member);

                memberMap.Name = _userRepository.FirstOrDefault(x => x.Id == member.UserID).Name;
                memberMap.Created = _issueRepository.Count(x => x.ProjectID == Id && x.CreatorId == member.UserID && x.CreationTime.AddDays(lastDay).Date >= Now.Date);
                memberMap.Closed = (from project in _projectRepository
                                    join issue in _issueRepository on project.Id equals issue.ProjectID
                                    join assignee in _assigneeRepository on issue.Id equals assignee.IssueID
                                    join status in _statusRepository on issue.StatusID equals status.Id
                                    where project.Id == Id && status.Name == "Closed" && assignee.UserID == member.UserID && issue.LastModificationTime.Value.AddDays(lastDay).Date >= Now.Date
                                    select new { issue }).Count();

                member1StatisticDto.Add(memberMap);
            }

            return new Project1StatisticDto
            {
                Created = Created,
                Closed = Closed,
                pCreated = pCreated,
                pClosed = pClosed,
                member1StatisticDtos = member1StatisticDto
            };
        }
        private string GetIcon(string type)
        {
            return "fas fa-" + type;
        }
        public async Task<ProjectTfsDto> GetProjectTfsDto(Guid Id)
        {
            string icon = "";
            var project = await _projectRepository.GetAsync(Id);
            var projectMap = ObjectMapper.Map<Project, ProjectTfsDto>(project);
            if (projectMap.WitType == "Bug")
            {
                icon = GetIcon("bug");
            }
            else if (projectMap.WitType == "Epic") icon = GetIcon("crown");
            else if (projectMap.WitType == "Feature") icon = GetIcon("trophy");
            else if (projectMap.WitType == "Issue") icon = GetIcon("disease");
            else if (projectMap.WitType == "Task") icon = GetIcon("clipboard-list");
            else if (projectMap.WitType == "Test Case") icon = GetIcon("vials");
            else if (projectMap.WitType == "User Story") icon = GetIcon("book-open");
            projectMap.Icon = icon;
            return projectMap;
        }

        public async Task<ProjectStatisticDto> ProjectStatisticAsync(string name, string CurrentUserId)
        {
            List<string> projectList = new List<string>();
            List<int> issuesClosed = new List<int>();
            List<int> issuesCreated = new List<int>();
            List<int> issuesOverDueDate = new List<int>();

            var Today = DateTime.UtcNow;
            if (name.IsNullOrWhiteSpace() || name == "null")
            {
                name = "";
            }
            var projects = from project in _projectRepository
                           join member in _memberRepository on project.Id equals member.ProjectID
                           where project.Name.Contains(name) && member.UserID == CurrentUserId
                           select new { project };
            var queryProjectResult = await AsyncExecuter.ToListAsync(projects);
            for (var i = 0; i < queryProjectResult.Count(); i++)
            {
                projectList.Add(queryProjectResult[i].project.Name);
                issuesClosed.Add((from project in _projectRepository
                                  join member in _memberRepository on project.Id equals member.ProjectID
                                  join issue in _issueRepository on project.Id equals issue.ProjectID
                                  join status in _statusRepository on issue.StatusID equals status.Id
                                  where project.Id == queryProjectResult[i].project.Id && status.Name == "Closed" && member.UserID == CurrentUserId
                                  select new { issue }).Count());
                issuesCreated.Add((from project in _projectRepository
                                   join member in _memberRepository on project.Id equals member.ProjectID
                                   join issue in _issueRepository on project.Id equals issue.ProjectID
                                   where project.Id == queryProjectResult[i].project.Id && member.UserID == CurrentUserId
                                   select new { issue }).Count());
                issuesOverDueDate.Add((from project in _projectRepository
                                       join member in _memberRepository on project.Id equals member.ProjectID
                                       join issue in _issueRepository on project.Id equals issue.ProjectID
                                       join status in _statusRepository on issue.StatusID equals status.Id
                                       where project.Id == queryProjectResult[i].project.Id && member.UserID == CurrentUserId && issue.DueDate <= Today && status.Name != "Closed"
                                       select new { issue }).Count());
            }
            int users = (from project in _projectRepository
                         join member in _memberRepository on project.Id equals member.ProjectID
                         join user in _userRepository on member.UserID equals user.Id
                         join assignee in _assigneeRepository on user.Id equals assignee.UserID
                         join issue in _issueRepository on assignee.IssueID equals issue.Id
                         join status in _statusRepository on issue.StatusID equals status.Id
                         where project.Name.Contains(name) && status.Name != "Closed" && _memberRepository.Any(x => x.ProjectID == project.Id && x.UserID == CurrentUserId)
                         select new { user }).Distinct().Count();
            int bugs = (from project in _projectRepository
                        join member in _memberRepository on project.Id equals member.ProjectID
                        join issue in _issueRepository on project.Id equals issue.ProjectID
                        where project.Name.Contains(name) && member.UserID == CurrentUserId
                        select new { issue }).Count();
            int totalBugs = _issueRepository.Count();
            int close = (from project in _projectRepository
                         join member in _memberRepository on project.Id equals member.ProjectID
                         join issue in _issueRepository on project.Id equals issue.ProjectID
                         join status in _statusRepository on issue.StatusID equals status.Id
                         where project.Name.Contains(name) && status.Name == "Closed" && member.UserID == CurrentUserId
                         select new { issue }).Count();
            int notClose = (from project in _projectRepository
                            join member in _memberRepository on project.Id equals member.ProjectID
                            join issue in _issueRepository on project.Id equals issue.ProjectID
                            join status in _statusRepository on issue.StatusID equals status.Id
                            where project.Name.Contains(name) && status.Name != "Closed" && status.Name != "Open" && member.UserID == CurrentUserId
                            select new { issue }).Count();
            int open = (from project in _projectRepository
                        join member in _memberRepository on project.Id equals member.ProjectID
                        join issue in _issueRepository on project.Id equals issue.ProjectID
                        join status in _statusRepository on issue.StatusID equals status.Id
                        where project.Name.Contains(name) && status.Name == "Open" && member.UserID == CurrentUserId
                        select new { issue }).Count();
            int tags = (from project in _projectRepository
                        join member in _memberRepository on project.Id equals member.ProjectID
                        join issue in _issueRepository on project.Id equals issue.ProjectID
                        join assign in _assigneeRepository on issue.Id equals assign.IssueID
                        where project.Name.Contains(name) && assign.UserID == CurrentUserId && member.UserID == CurrentUserId
                        select new { issue }).Count();
            int create = (from project in _projectRepository
                          join member in _memberRepository on project.Id equals member.ProjectID
                          join issue in _issueRepository on project.Id equals issue.ProjectID
                          where project.Name.Contains(name) && issue.CreatorId == CurrentUserId && member.UserID == CurrentUserId
                          select new { issue }).Count();
            int dueDate = (from project in _projectRepository
                           join member in _memberRepository on project.Id equals member.ProjectID
                           join issue in _issueRepository on project.Id equals issue.ProjectID
                           join status in _statusRepository on issue.StatusID equals status.Id
                           where project.Name.Contains(name) && issue.DueDate <= Today && status.Name != "Closed" && member.UserID == CurrentUserId
                           select new { issue }).Count();
            int comment = (from project in _projectRepository
                           join member in _memberRepository on project.Id equals member.ProjectID
                           join issue in _issueRepository on project.Id equals issue.ProjectID
                           join com in _commentRepository on issue.Id equals com.IssueID
                           where project.Name.Contains(name) && member.UserID == CurrentUserId
                           select new { issue }).Count();
            return new ProjectStatisticDto
            {
                Project = projects.Count(),
                User = users,
                Bugs = bugs,
                TotalBug = totalBugs,
                Close = close,
                NotClose = notClose,
                Tag = tags,
                Create = create,
                DueDate = dueDate,
                Comment = comment,
                Open = open,
                Projects = projectList.ToArray(),
                IssuesCreated = issuesCreated.ToArray(),
                IssuesClosed = issuesClosed.ToArray(),
                IssuesOverDueDate = issuesOverDueDate.ToArray()

            };
        }

        public List<UserProcessingDto> GetListUserProcessing()
        {
            string currentUserId = _tmtCurrentUser.GetId();
            var usersProcessingDto = new List<UserProcessingDto>();
            var members = from project in _projectRepository
                          join member in _memberRepository on project.Id equals member.ProjectID
                          join user in _userRepository on member.UserID equals user.Id
                          join assignee in _assigneeRepository on user.Id equals assignee.UserID
                          join issue in _issueRepository on assignee.IssueID equals issue.Id
                          join status in _statusRepository on issue.StatusID equals status.Id
                          where status.Name != "Closed" && _memberRepository.Any(x => x.ProjectID == project.Id && x.UserID == currentUserId)
                          select new { user, member };

            var membersList = members.ToList().Distinct();

            foreach (var member in membersList)
            {
                var userProcessingDto = new UserProcessingDto();
                var projects = (from project in _projectRepository
                                join memberJ in _memberRepository on project.Id equals memberJ.ProjectID
                                join user in _userRepository on memberJ.UserID equals user.Id
                                join assignee in _assigneeRepository on user.Id equals assignee.UserID
                                join issue in _issueRepository on project.Id equals issue.ProjectID
                                where memberJ.UserID == member.user.Id && assignee.UserID == member.user.Id && assignee.UserID == member.user.Id && assignee.IssueID == issue.Id
                                select new { project }).Distinct().ToList();
                userProcessingDto.Id = member.user.Id;
                userProcessingDto.Name = member.user.Name;

                userProcessingDto.Bugs = (from project in _projectRepository
                                          join memberJ in _memberRepository on project.Id equals memberJ.ProjectID
                                          join user in _userRepository on memberJ.UserID equals user.Id
                                          join assignee in _assigneeRepository on user.Id equals assignee.UserID
                                          join issue in _issueRepository on project.Id equals issue.ProjectID
                                          where memberJ.UserID == member.user.Id && assignee.UserID == member.user.Id && assignee.UserID == member.user.Id && assignee.IssueID == issue.Id
                                          select new { issue }).Count();

                foreach (var project in projects)
                {
                    userProcessingDto.Projects += project.project.Name + " / ";
                }

                if (!usersProcessingDto.Any(x => x.Id == member.member.UserID))
                {
                    usersProcessingDto.Add(userProcessingDto);
                }

            }

            return usersProcessingDto;
        }

        public List<UserProcessingDetailDto> GetUserProcessingDetails(string userId)
        {
            var userProcessingDetails = new List<UserProcessingDetailDto>();
            var projects = from project in _projectRepository
                           join member in _memberRepository on project.Id equals member.ProjectID
                           join user in _userRepository on member.UserID equals user.Id
                           join assignee in _assigneeRepository on user.Id equals assignee.UserID
                           join issue in _issueRepository on assignee.IssueID equals issue.Id
                           join status in _statusRepository on issue.StatusID equals status.Id
                           where status.Name != "Closed" && _memberRepository.Any(x => x.ProjectID == project.Id && x.UserID == userId) && assignee.UserID == userId
                           select new { project };

            var projectsList = projects.ToList().Distinct();
            foreach (var project in projectsList)
            {
                var userProcessingDetail = new UserProcessingDetailDto();

                userProcessingDetail.Id = project.project.Id;
                userProcessingDetail.Name = project.project.Name;

                userProcessingDetail.Bugs = (from projectJ in _projectRepository
                                             join memberJ in _memberRepository on projectJ.Id equals memberJ.ProjectID
                                             join issue in _issueRepository on projectJ.Id equals issue.ProjectID
                                             join user in _userRepository on memberJ.UserID equals user.Id
                                             join assignee in _assigneeRepository on user.Id equals assignee.UserID
                                             where projectJ.Id == project.project.Id && memberJ.UserID == userId && assignee.UserID == userId && assignee.UserID == userId && assignee.IssueID == issue.Id
                                             select new { issue }).Count();

                var issues = (from projectJ in _projectRepository
                              join memberJ in _memberRepository on projectJ.Id equals memberJ.ProjectID
                              join user in _userRepository on memberJ.UserID equals user.Id
                              join assignee in _assigneeRepository on user.Id equals assignee.UserID
                              join issue in _issueRepository on projectJ.Id equals issue.ProjectID
                              where projectJ.Id == project.project.Id && memberJ.UserID == userId && assignee.UserID == userId && assignee.IssueID == issue.Id
                              select new { issue }).ToList();

                foreach (var issue in issues)
                {
                    userProcessingDetail.BugsName += issue.issue.Name + " / ";
                }

                if (userProcessingDetail.Bugs > 0)
                {
                    userProcessingDetails.Add(userProcessingDetail);
                }

            }

            return userProcessingDetails;
        }
        [AllowAnonymous]
        public async Task<List<WorkItem>> GetWitsByWiql(string url, string pAT, string project, string title, string types, string states, ConditionType conditionType)
        {
            return await _azureAppService.GetWitsByWiql(url, pAT, project, title, types, states, conditionType);
        }



    }
}
