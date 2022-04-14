using BugTracking.Assignees;
using BugTracking.Categories;
using BugTracking.Departments;
using BugTracking.Follows;
using BugTracking.Histories;
using BugTracking.HistoryDashboards;
using BugTracking.HistoryViews;
using BugTracking.IShareDto;
using BugTracking.Issues;
using BugTracking.MemberTeams;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Teams;
using BugTracking.Users;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AuditLogging;
using Volo.Abp.Domain.Entities;

namespace BugTracking.History
{
    [Authorize("BugTracking.Users")]
    public class HistoryService : BugTrackingAppService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IEnityChangeRepository _enityChangeRepository;
        private readonly IEnityPropertyRepository _entityPropertyChangeRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IFollowRepository _followRepository;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly IAssigneeRepository _assignRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IHistoryViewRepository _historyRepository;
        private readonly IHistoryDashboardRepository _historyDashboardRepository;
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "GoogleSheetsHelper";
        private SheetsService _sheetsService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IMemberTeamRepository _memberTeamRepository;
        public readonly StatusService _statusService;

        public HistoryService(IAuditLogRepository auditLogRepository, 
            IEnityChangeRepository enityChangeRepository, 
            IEnityPropertyRepository entityPropertyChangeRepository,
            IStatusRepository statusRepository, 
            IProjectRepository projectRepository, 
            IIssueRepository issueRepository, 
            IFollowRepository followRepository, 
            ITMTCurrentUser tmtCurrentUser,
            IAssigneeRepository assignRepository, 
            UserManager userManager, 
            IUserRepository userRepository, 
            ICategoryRepository categoryRepository, 
            IBackgroundJobClient backgroundJobClient ,
            IHistoryViewRepository historyRepository, 
            IDepartmentRepository departmentRepository , 
            ITeamRepository teamRepository, 
            IMemberTeamRepository memberTeamReoprository,
            StatusService statusService,
            IHistoryDashboardRepository historyDashboardRepository)
        {
            _statusRepository = statusRepository;
            _auditLogRepository = auditLogRepository;
            _enityChangeRepository = enityChangeRepository;
            _entityPropertyChangeRepository = entityPropertyChangeRepository;
            _projectRepository = projectRepository;
            _issueRepository = issueRepository;
            _followRepository = followRepository;
            _tmtCurrentUser = tmtCurrentUser;
            _assignRepository = assignRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _backgroundJobClient = backgroundJobClient;
            _historyRepository = historyRepository;
            _departmentRepository = departmentRepository;
            _teamRepository = teamRepository;
            _memberTeamRepository = memberTeamReoprository;
            _statusService = statusService;
            _historyDashboardRepository = historyDashboardRepository;
        }


        public class CountAndResult
        {
            public int count { get; set; }
            public Dictionary<string, List<HistoryDTO>> result { get; set; }
        }
       
        public class HistoryCompare : IEqualityComparer<HistoryDTO>
        {
            public bool Equals(HistoryDTO x, HistoryDTO y)
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
                return x.AuditLogId == y.AuditLogId;
            }

            public int GetHashCode([DisallowNull] HistoryDTO obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                //Get the ID hash code value
                int IDHashCode = obj.AuditLogId.GetHashCode();

                //Get the string HashCode Value
                //Check for null refernece exception

                return IDHashCode;
            }
        }
        public async Task<List<HistoryViewDto>> GetListAsync()
        {
            var history = await _historyRepository.GetListAsync();

            return new List<HistoryViewDto>(
               ObjectMapper.Map<List<HistoryView>, List<HistoryViewDto>>(history)
           );
        }
        public async Task<List<HistoryDashboardDto>> GetListHistoryDashboard()
        {
            var history = await _historyDashboardRepository.GetListAsync();

            return new List<HistoryDashboardDto>(
               ObjectMapper.Map<List<HistoryDashboard>, List<HistoryDashboardDto>>(history)
           );
        }
        public async Task<CountAndResult> GetListResultAsync(GetListDto input, Guid projectId, DateTime? startDate, DateTime? endDate, string idUser, string acc, string issueId)
        {

             var CurrentUserId = _tmtCurrentUser.GetId();
           // var CurrentUserId = "1439804071860083";
            HistoryCompare historyCompare = new HistoryCompare();
            if (idUser.IsNullOrWhiteSpace() || idUser == "null")
            {
                idUser = "";
            }
            if (acc.IsNullOrWhiteSpace() || acc == "null")
            {
                acc = "";
            }
            if (issueId.IsNullOrWhiteSpace() || issueId == "null")
            {
                issueId = "";
            }
            var isAdminOrProjectCreator = await _userManager.CheckRoleByUserId(CurrentUserId, "admin") || _projectRepository.Any(x => x.Id == projectId && x.CreatorId == CurrentUserId);
            var statusQuery = await _statusRepository.IgnoreQueryFilters().ToListAsync();

            var Assign = (from viewHistory in _historyRepository
                          join issue in _issueRepository on viewHistory.EntityId equals issue.Id.ToString()
                          join assignee in _assignRepository on issue.Id equals assignee.IssueID
                          where issue.ProjectID == projectId && assignee.UserID == CurrentUserId
                           && (!idUser.IsNullOrEmpty() ? viewHistory.UserId == idUser : true)
                           && (!acc.IsNullOrEmpty() ? viewHistory.HttpMethod == acc : true)
                           && (startDate.HasValue ? viewHistory.ExecutionTime >= startDate.GetValueOrDefault() : true)
                           && (endDate.HasValue ? viewHistory.ExecutionTime <= endDate.GetValueOrDefault() : true)
                           && (!issueId.IsNullOrEmpty() ? issue.Id.ToString() == issueId : true)
                          orderby viewHistory.ExecutionTime descending
                          select new HistoryDTO
                          {
                              IdProject = issue.ProjectID,
                              IssueId = issue.Id,
                              UserName = viewHistory.UserName,
                              Action = (viewHistory.HttpMethod == "POST" ? "CREATED" : viewHistory.HttpMethod == "PUT" ? "UPDATE" : viewHistory.HttpMethod),
                              OldValue = viewHistory.OriginalValue != null ? _statusRepository.FirstOrDefault(x => viewHistory.OriginalValue.Contains(x.Id.ToString())).Name : null,
                              NewValue = viewHistory.NewValue != null ? _statusRepository.FirstOrDefault(s => viewHistory.NewValue.Contains(s.Id.ToString())).Name : null,
                              Entity = issue.Name,
                              AuditLogId = viewHistory.Id,
                              ExcutitonTime = viewHistory.ExecutionTime,
                              Color = (viewHistory.HttpMethod == "POST" ? "#06ab12" : viewHistory.HttpMethod == "PUT" ? "#1792da" : "#da1717"),
                              nzColor = viewHistory.NewValue != null ? _statusRepository.FirstOrDefault(s => viewHistory.NewValue.Contains(s.Id.ToString())).NzColor : null,
                          }
                          );

            var Creator = (from viewHistory in _historyRepository
                           join issue in _issueRepository on viewHistory.EntityId equals issue.Id.ToString()
                           where  issue.ProjectID == projectId
                            && (!idUser.IsNullOrEmpty() ? viewHistory.UserId == idUser : true)
                            && (!acc.IsNullOrEmpty() ? viewHistory.HttpMethod == acc : true)
                            && (startDate.HasValue ? viewHistory.ExecutionTime >= startDate.GetValueOrDefault() : true)
                            && (endDate.HasValue ? viewHistory.ExecutionTime <= endDate.GetValueOrDefault() : true)
                            && issue.CreatorId == CurrentUserId
                            && (!issueId.IsNullOrEmpty() ? issue.Id.ToString() == issueId : true)
                           orderby viewHistory.ExecutionTime descending
                           select new HistoryDTO
                           {
                               IdProject = issue.ProjectID,
                               IssueId = issue.Id,
                               UserName = viewHistory.UserName,
                               Action = (viewHistory.HttpMethod == "POST" ? "CREATED" : viewHistory.HttpMethod == "PUT" ? "UPDATE" : viewHistory.HttpMethod),
                               Entity = issue.Name,
                               OldValue = viewHistory.OriginalValue != null ? _statusRepository.FirstOrDefault(x => viewHistory.OriginalValue.Contains(x.Id.ToString())).Name : null,
                               NewValue = viewHistory.NewValue != null ? _statusRepository.FirstOrDefault(s => viewHistory.NewValue.Contains(s.Id.ToString())).Name : null,
                               AuditLogId = viewHistory.Id,
                               ExcutitonTime = viewHistory.ExecutionTime,
                               Color = (viewHistory.HttpMethod == "POST" ? "#06ab12" : viewHistory.HttpMethod == "PUT" ? "#1792da" : "#da1717"),
                               nzColor = viewHistory.NewValue != null ? _statusRepository.FirstOrDefault(s => viewHistory.NewValue.Contains(s.Id.ToString())).NzColor : null,
                           }
                           );

            var Admin = (from viewHistory in _historyRepository.IgnoreQueryFilters()
                         join issue in _issueRepository on viewHistory.EntityId equals issue.Id.ToString()
                         where  issue.ProjectID == projectId &&
                          (!idUser.IsNullOrEmpty() ? viewHistory.UserId == idUser : true)
                          && (!acc.IsNullOrEmpty() ? viewHistory.HttpMethod == acc : true)
                          && (startDate.HasValue ? viewHistory.ExecutionTime >= startDate.GetValueOrDefault() : true)
                          && (endDate.HasValue ? viewHistory.ExecutionTime <= endDate.GetValueOrDefault() : true)
                          && (!issueId.IsNullOrEmpty() ? issue.Id.ToString() == issueId : true)
                         orderby viewHistory.ExecutionTime descending
                         select new HistoryDTO
                         {
                             IdProject = issue.ProjectID,
                             IssueId = issue.Id,
                             UserName = viewHistory.UserName,
                             Action = (viewHistory.HttpMethod == "POST" ? "CREATED" : viewHistory.HttpMethod == "PUT" ? "UPDATE" : viewHistory.HttpMethod),
                             Entity = issue.Name,
                             OldValue = viewHistory.OriginalValue != null ? _statusRepository.FirstOrDefault(x => viewHistory.OriginalValue.Contains(x.Id.ToString())).Name : null,
                             NewValue = viewHistory.NewValue != null ? _statusRepository.FirstOrDefault(s => viewHistory.NewValue.Contains(s.Id.ToString())).Name : null,
                             AuditLogId = viewHistory.Id,
                             ExcutitonTime = viewHistory.ExecutionTime,
                             Color = (viewHistory.HttpMethod == "POST" ? "#06ab12" : viewHistory.HttpMethod == "PUT" ? "#1792da" : "#da1717"),
                             nzColor = viewHistory.NewValue != null ? _statusRepository.FirstOrDefault(s => viewHistory.NewValue.Contains(s.Id.ToString())).NzColor : null,
                         }
              );

            var queryResult = isAdminOrProjectCreator ? Admin : (Creator.Union(Assign));
            int count = queryResult.Count();
            var result = queryResult.Skip(input.SkipCount).Take(input.MaxResultCount).ToList().OrderByDescending(x=>x.ExcutitonTime);
            var newGroup = result.GroupBy(x => x.ExcutitonTime.ToString("yyyy-MM-dd")).OrderByDescending(x=>x.Key);
            Dictionary<string, List<HistoryDTO>> listHistory = new Dictionary<string, List<HistoryDTO>>();
            foreach (var group in newGroup)
            {
                listHistory.Add(group.Key, group.OrderByDescending(x=>x.ExcutitonTime).ToList());
            }
            return new CountAndResult
            {
                count = count,
                result = listHistory
            };
        }

        public List<TeamFollowHistoryDto> GetListTeamByUserId()
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var teamQuery = from team in _teamRepository
                            join member in _memberTeamRepository on team.Id equals member.IdTeam
                            where member.UserID == CurrentUserId
                            select new TeamFollowHistoryDto
                            {  
                                nameTeam = team.Name,
                                userId = member.UserID,
                                teamId = team.Id.ToString()
                            };
            var result = teamQuery.ToList();
            return result;
                            
        }
        public List<CalendarViewDto> GetHistoryFollow(string isssueName, string idProject, string status, string team , string actionFollow)
        {
            if (isssueName.IsNullOrWhiteSpace() || isssueName == "null")
            {
                isssueName = "";
            }
            if (idProject.IsNullOrWhiteSpace() || idProject == "null")
            {
                idProject = "";
            }
            if (status.IsNullOrWhiteSpace() || status == "null")
            {
                status = "";
            }
            if (team.IsNullOrWhiteSpace() || team == "null")
            {
                team = "";
            }
            var CurrentUserId = _tmtCurrentUser.GetId();
            var listCalendarViewDto = new List<CalendarViewDto>();
            var followAssignee = from issue in _issueRepository
                         join fl in _followRepository on issue.Id equals fl.IssueID
                         join assignee in _assignRepository on fl.IssueID equals assignee.IssueID
                         where fl.UserID == CurrentUserId && assignee.UserID== CurrentUserId &&(!isssueName.IsNullOrEmpty() ? issue.Name == isssueName : true)
                               && (!idProject.IsNullOrEmpty() ? issue.ProjectID.ToString() == idProject : true)
                               && (!status.IsNullOrEmpty() ? issue.StatusID.ToString() == status : true)
                               && (!team.IsNullOrEmpty() ? CurrentUserId == team : true)
                         select new { issue };
            var  followCreater = (from issue in _issueRepository
                                  join fl in _followRepository on issue.Id equals fl.IssueID
                                  where fl.UserID == CurrentUserId && (!isssueName.IsNullOrEmpty() ? issue.Name == isssueName : true)
                                  && (!idProject.IsNullOrEmpty() ? issue.ProjectID.ToString() == idProject : true)
                                  && (!status.IsNullOrEmpty() ? issue.StatusID.ToString() == status : true)
                                  && (!team.IsNullOrEmpty() ? CurrentUserId == team : true)
                                  && issue.CreatorId == CurrentUserId
                                  orderby issue.StartDate
                                  select new { issue });
            var query = (from issue in _issueRepository
                         join fl in _followRepository on issue.Id equals fl.IssueID
                         where fl.UserID == CurrentUserId && (!isssueName.IsNullOrEmpty() ? issue.Name == isssueName : true) 
                         && (!idProject.IsNullOrEmpty() ? issue.ProjectID.ToString() == idProject : true)
                         && (!status.IsNullOrEmpty() ? issue.StatusID.ToString() == status : true)
                         && (!team.IsNullOrEmpty() ? CurrentUserId == team : true)
                         orderby issue.StartDate
                         select new { issue });
            var queryResult = actionFollow == "creater" ? followCreater : actionFollow == "assignee" ? followAssignee.Distinct() : query;
            var result = queryResult.OrderByDescending(x=>x.issue.StartDate).ToList();
            for (var i = 0; i < result.Count; i++)
            {
                DateTime dtNow = DateTime.Now;
                var calendarViewDto = new CalendarViewDto();
                
                calendarViewDto.start = (DateTime)result[i].issue.StartDate;
                calendarViewDto.IssueId = result[i].issue.Id;
                if(result[i].issue.DueDate != null && dtNow > result[i].issue.DueDate)
                {
                    calendarViewDto.title = result[i].issue.Name + "--" + _statusRepository.FirstOrDefault(s => result[i].issue.StatusID.ToString().Contains(s.Id.ToString())).Name + "(Out of date)";
                }
                else
                {
                    calendarViewDto.title = result[i].issue.Name + "--" + _statusRepository.FirstOrDefault(s => result[i].issue.StatusID.ToString().Contains(s.Id.ToString())).Name;
                }
                if (result[i].issue.FinishDate == null)
                {
                    TimeSpan timeBug = dtNow - (DateTime)result[i].issue.StartDate;
                    if(timeBug > TimeSpan.FromDays(1)){
                        calendarViewDto.allDay = true;
                    }
                    else
                    {
                        calendarViewDto.allDay = false;
                    }
                    calendarViewDto.end = dtNow;
                }
                else
                {

                    TimeSpan timeBug = (DateTime)result[i].issue.FinishDate - (DateTime)result[i].issue.StartDate;
                    if (timeBug > TimeSpan.FromDays(1))
                    {
                        calendarViewDto.allDay = true;
                    }
                    else
                    {
                        calendarViewDto.allDay = false;
                    }
                    calendarViewDto.end = (DateTime)result[i].issue.FinishDate;
                }
                calendarViewDto.color = _statusRepository.FirstOrDefault(s => result[i].issue.StatusID.ToString().Contains(s.Id.ToString())).NzColor;
                listCalendarViewDto.Add(calendarViewDto);
            }

            return listCalendarViewDto;
        }
        public ListResultDto<Users.UserDto> GetListUserAuditLogs(Guid projectId)
        {
            //Create a query
            var userQuery = _userRepository.ToList();
            var query = from user in userQuery
                        join audit in _auditLogRepository on user.Id equals audit.UserId
                        join entityChange in _enityChangeRepository on audit.Id equals entityChange.AuditLogId
                        join issue in _issueRepository on entityChange.EntityId equals issue.Id.ToString()
                        where issue.ProjectID == projectId
                        select user;

            //Execute the query to get list of people
            var users = query.Distinct().ToList();

            //Convert to DTO and return to the client
            return new ListResultDto<Users.UserDto>(ObjectMapper.Map<List<AppUser>, List<Users.UserDto>>(users));
        }
        public List<AssigneeByUserDto> GetListIssueFollow(string idProject)
        {
            if (idProject.IsNullOrWhiteSpace() || idProject == "null")
            {
                idProject = "";
            }
            var CurrentUserId = _tmtCurrentUser.GetId();
            var query = from issue in _issueRepository
                        join follow in _followRepository on issue.Id equals follow.IssueID
                        where follow.UserID == CurrentUserId && (!idProject.IsNullOrEmpty() ? issue.ProjectID.ToString() == idProject : true)
                        select new AssigneeByUserDto
                        {
                            idIssue = issue.Id,
                            IssueName = issue.Name
                        };
            var result = query.ToList();
            return result;
        }
        public async Task<IssueDetailCalendar> GetIssueDetail(Guid id)
        {
            var query = from issue in _issueRepository
                        join project in _projectRepository on issue.ProjectID equals project.Id
                        join status in _statusRepository on issue.StatusID equals status.Id
                        where issue.Id == id
                        select new { issue, project , status };

            var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);

            if (queryResult == null)
            {
                throw new EntityNotFoundException(typeof(Issue), id);
            }

            var calendar = ObjectMapper.Map<Issue, IssueDetailCalendar>(queryResult.issue);

            calendar.ProjectName = queryResult.project.Name;
            calendar.status = queryResult.status.Name;
            calendar.ListAssignee = _statusService.GetAssigneeDtos(queryResult.issue.Id, "");

            return calendar;
        }
        public async Task<Dictionary<string, List<HistoryDTO>>> GetHistoryByIssue(Guid id)
        {

            var auditLogQuery = await _auditLogRepository.IgnoreQueryFilters().ToListAsync();
            var statusQuery = await _statusRepository.IgnoreQueryFilters().ToListAsync();

            var query = (from audit in auditLogQuery
                         join entityChange in _enityChangeRepository.IgnoreQueryFilters() on audit.Id equals entityChange.AuditLogId
                         join issue in _issueRepository.IgnoreQueryFilters() on entityChange.EntityId equals issue.Id.ToString()
                         join entityPropertyChange in _entityPropertyChangeRepository.IgnoreQueryFilters() on entityChange.Id equals entityPropertyChange.EntityChangeId
                         where (entityPropertyChange.PropertyName == "StatusID"|| entityPropertyChange.PropertyName == "BugTracking.Comments.Comment") && issue.Id == id
                         orderby audit.ExecutionTime descending
                         select new HistoryDTO
                         {
                             AuditLogId = audit.Id,
                             UserName = audit.UserName,
                             Action = (audit.HttpMethod == "POST" ? "CREATED" : audit.HttpMethod == "PUT" ? "UPDATE" : audit.HttpMethod),

                             Entity = issue.Name,
                             OldValue = statusQuery.Find(s => s.Id.ToString() == entityPropertyChange.OriginalValue?.Replace("\"", ""))?.Name,
                             NewValue = statusQuery.Find(s => s.Id.ToString() == entityPropertyChange.NewValue?.Replace("\"", ""))?.Name,
                             ExcutitonTime = audit.ExecutionTime,
                             Color = (audit.HttpMethod == "POST" ? "#06ab12" : audit.HttpMethod == "PUT" ? "#1792da" : "#da1717"),
                         }
                          );

            var queryResult = query.ToList();
            var newGroup = queryResult.GroupBy(x => x.ExcutitonTime.ToString("yyyy-MM-dd"));
            Dictionary<string, List<HistoryDTO>> listHistory = new Dictionary<string, List<HistoryDTO>>();
            foreach (var group in newGroup)
            {
                listHistory.Add(group.Key, group.ToList());
            }
            return listHistory;
        }
        public async Task<Spreadsheet> GetNewGoogleSheetAsync()
        {
            GetCredential();
            Spreadsheet requestBody = new Spreadsheet();
            SpreadsheetsResource.CreateRequest request = _sheetsService.Spreadsheets.Create(requestBody);
            Spreadsheet response = request.Execute();
            await AddRowSheetAsync(response.SpreadsheetId, response.Sheets[0].Properties.Title);
            return response;
        }
        public async Task GetUpdateGoogleSheetAsync(string spreadsheetId, string sheet)
        {
            GetCredential();
            await ReadSheetAsync(spreadsheetId, sheet);
        }
        private void GetCredential()
        {
            UserCredential credential;
            using (var stream =
                new FileStream("credential.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None).Result;
            }
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }
        private async Task ReadSheetAsync(string spreadsheetId, string sheet)
        {
            try
            {
            var range = $"{sheet}!A:I";
            ClearValuesRequest requestBody = new ClearValuesRequest();
            SpreadsheetsResource.ValuesResource.ClearRequest requestClear = _sheetsService.Spreadsheets.Values.Clear(requestBody, spreadsheetId, range);
            requestClear.Execute();
            await AddRowSheetAsync(spreadsheetId,sheet);
            }
            catch
            {
                throw new UserFriendlyException("Cannot write on SpreadSheetId or SheetName, please check your input !!!");
            }
        }
        private async Task AddRowSheetAsync(string spreadsheetId, string sheet)
        {
            var projects = await _projectRepository.GetListAsync();
            foreach (Project project in projects)
            {
                if (_issueRepository.Any(x => x.ProjectID == project.Id))
                {
                    AddRow(spreadsheetId, sheet,"PROJECT: " + project.Name, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                    AddRow(spreadsheetId, sheet, "Name", "Description", "Category", "Start Date", "Due Date", "Finish Date", "Status", "Fisnish");
                    var issues = await _issueRepository.GetListAsync(x => x.ProjectID == project.Id);
                    foreach (Issue issue in issues)
                    {
                        var status = _statusRepository.FirstOrDefault(x => x.Id == issue.StatusID).Name;
                        AddRow(spreadsheetId,
                            sheet,
                            issue.Name,
                            HttpUtility.HtmlDecode(issue.Description),
                            issue.CategoryID != null ? _categoryRepository.FirstOrDefault(x => x.Id == issue.CategoryID).Name : null,
                            issue.StartDate.ToString(), issue.DueDate != null ? issue.DueDate.ToString() : null, issue.FinishDate != null ? issue.FinishDate.ToString() : null,
                            status,
                            status == "Closed" ? "true" : "false");
                    }
                    AddRow(spreadsheetId, sheet, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                }
            }
        }
        public static async Task<string> Http1_Get_With_Proxy(string url1)
        {
            string html_content = "";
            try
            {
                WebProxy proxy = new WebProxy
                {
                    Address = new Uri($"http://1.2.3.4:8888"),
                };
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                HttpClientHandler clientHandler = new HttpClientHandler()
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    Proxy = proxy,
                };
                using HttpClient client1 = new HttpClient(clientHandler);
                client1.DefaultRequestHeaders.Accept.Clear();
                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                client1.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url1);
                HttpResponseMessage response = await client1.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    html_content = await response.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (html_content);
        }
        private void AddRow(string spreadsheetId,string sheet,string name, string des,string category, string startDate, string dueDate,string finishDate, string status, string isFinish)
        {
            var range = $"{sheet}!A:I";
            var valueRange = new ValueRange();
            var oblist = new List<object>() { name, des, category, startDate, dueDate, finishDate, status, isFinish };
            valueRange.Values = new List<IList<object>> { oblist };
            var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            appendRequest.Execute();
        }

    }

}
