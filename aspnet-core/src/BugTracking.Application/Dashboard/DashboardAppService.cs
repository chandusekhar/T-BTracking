using BugTracking.Assignees;
using BugTracking.Dashboards;
using BugTracking.HistoryDashboards;
using BugTracking.HistoryViews;
using BugTracking.IShareDto;
using BugTracking.Issues;
using BugTracking.Members;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AuditLogging;
using static BugTracking.Dashboards.DashboardDto;

namespace BugTracking.Dashboard
{
    [Authorize("BugTracking.Users")]
    public class DashboardAppService : BugTrackingAppService, IDashboardService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IEnityChangeRepository _enityChangeRepository;
        private readonly IEnityPropertyRepository _entityPropertyChangeRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly IAssigneeRepository _assignRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager _userManager;
        private readonly IHistoryDashboardRepository _historyDashboardRepository;
        private readonly IHistoryViewRepository _historyViewReponsitory;

        public DashboardAppService(IUserRepository userRepository,
            IAuditLogRepository auditLogRepository,
            IEnityChangeRepository enityChangeRepository,
            IEnityPropertyRepository entityPropertyChangeRepository,
            IAssigneeRepository assignRepository,
            ITMTCurrentUser tMTCurrentuser,
            IStatusRepository statusRepository,
            IProjectRepository projectRepository,
            IIssueRepository issueRepository,
            UserManager userManager,
            IMemberRepository memberRepository,
         IHistoryDashboardRepository historyDashboardRepository,
         IHistoryViewRepository historyViewReponsitory
         )
        {
            _userRepository = userRepository;
            _auditLogRepository = auditLogRepository;
            _enityChangeRepository = enityChangeRepository;
            _entityPropertyChangeRepository = entityPropertyChangeRepository;
            _assignRepository = assignRepository;
            _tmtCurrentUser = tMTCurrentuser;
            _memberRepository = memberRepository;
            _statusRepository = statusRepository;
            _projectRepository = projectRepository;
            _issueRepository = issueRepository;
            _userManager = userManager;
            _historyDashboardRepository = historyDashboardRepository;
            _historyViewReponsitory = historyViewReponsitory;
        }
        public class HistoryCompare : IEqualityComparer<DBUpdateDto>
        {
            public bool Equals(DBUpdateDto x, DBUpdateDto y)
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

            public int GetHashCode([DisallowNull] DBUpdateDto obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                //Get the ID hash code value
                int IDHashCode = obj.Entity.GetHashCode();

                //Get the string HashCode Value
                //Check for null refernece exception

                return IDHashCode;
            }
        }
        public async Task<IEnumerable<ReturnUpdatesDto>> GetListByUserId(int skipCount,int pageSize, string userId, string projectName)
        {
            if (projectName.IsNullOrWhiteSpace() || projectName == "null")
            {
                projectName = "";
            }
            HistoryCompare historyCompare = new HistoryCompare();
            //var projectQuery = await _projectRepository.Where(x=>x.Name.Contains(projectName)).IgnoreQueryFilters().ToListAsync();
            //var memberQuery = await _memberRepository.IgnoreQueryFilters().ToListAsync();
            //var auditLogQuery = await _auditLogRepository.IgnoreQueryFilters().ToListAsync();
            //var issueQuery = await _issueRepository.IgnoreQueryFilters().ToListAsync();
            //var entityChangeQuery = await _enityChangeRepository.IgnoreQueryFilters().ToListAsync();
            var statusQuery = await _statusRepository.IgnoreQueryFilters().ToListAsync();
            //var entityPropertyChangeQuery = await _entityPropertyChangeRepository.IgnoreQueryFilters().ToListAsync();
            IEnumerable<ReturnUpdatesDto> returnData;
            //.Distinct(historyCompare).
            if (_userManager.CheckRoleByUserId(userId, "admin").Result)
            {
                var issues = (from audit in _historyViewReponsitory
                              join issue in _issueRepository.IgnoreQueryFilters() on audit.EntityId equals issue.Id.ToString()
                              join project in _projectRepository on issue.ProjectID equals project.Id
                              select new DBUpdateDto
                              {
                                  AuditLogId = audit.Id.ToString(),
                                  UserName = audit.UserName,
                                  Action = new
                                  {
                                      value = audit.HttpMethod,
                                      color = (audit.HttpMethod == "POST" ? "#06ab12" : audit.HttpMethod == "PUT" ? "#1792da" : "#da1717")
                                  },
                                  Entity = "issue " + issue.Name,
                                  InProject = project.Name,
                                  OldValue = audit.OriginalValue != null ? _statusRepository.FirstOrDefault(x => audit.OriginalValue.Contains(x.Id.ToString())).Name : null,
                                  NewValue = audit.NewValue != null ? _statusRepository.FirstOrDefault(s => audit.NewValue.Contains(s.Id.ToString())).Name : null,
                                  ExcutitonTime = audit.ExecutionTime
                              }
                         ).ToList();
                var projects = (from audit in _historyDashboardRepository
                                join issue in _issueRepository.IgnoreQueryFilters() on audit.EntityId equals issue.Id.ToString()
                                join project in _projectRepository on issue.ProjectID equals project.Id
                                select new DBUpdateDto
                                {
                                    UserName = audit.UserName,
                                    Action = new
                                    {
                                        value = audit.HttpMethod,
                                        color = (audit.HttpMethod == "POST" ? "#06ab12" : audit.HttpMethod == "PUT" ? "#1792da" : "#da1717")
                                    },
                                    Entity = "project " + project.Name,

                                    ExcutitonTime = audit.ExecutionTime,
                                }
                               ).ToList();

                issues.AddRange(projects);
                var result = issues.OrderByDescending(i => i.ExcutitonTime).ToList();
                var groupByDate = result.GroupBy(x => x.ExcutitonTime.ToString("yyyy-MM-dd"));
                returnData = groupByDate.Select(x =>
               {
                   var data = new ReturnUpdatesDto
                   {
                       Date = x.Key,
                       Data = x
                   };
                   return data;
               }).Skip(skipCount).Take(pageSize);
            }
            else
            {
                var issues = (from audit in _historyDashboardRepository
                              join issue in _issueRepository on audit.EntityId equals issue.Id.ToString()
                              join member in _memberRepository on issue.ProjectID equals member.ProjectID
                              join project in _projectRepository on member.ProjectID equals project.Id
                              where member.UserID == userId
                              select new DBUpdateDto
                              {
                                  AuditLogId = audit.Id.ToString(),
                                  UserName = audit.UserName,
                                  Action = new
                                  {
                                      value = audit.HttpMethod,
                                      color = (audit.HttpMethod == "POST" ? "#06ab12" : audit.HttpMethod == "PUT" ? "#1792da" : "#da1717")
                                  },
                                  Entity = "issue " + issue.Name,
                                  InProject = project.Name,
                                  OldValue = audit.OriginalValue != null ? _statusRepository.FirstOrDefault(x => audit.OriginalValue.Contains(x.Id.ToString())).Name : null,
                                  NewValue = audit.NewValue != null ? _statusRepository.FirstOrDefault(s => audit.NewValue.Contains(s.Id.ToString())).Name : null,
                                  ExcutitonTime = audit.ExecutionTime
                              }
                         );
                var projects = (from audit in _historyDashboardRepository
                                join member in _memberRepository on audit.EntityId equals member.ProjectID.ToString()
                                join project in _projectRepository on member.ProjectID equals project.Id
                                where member.UserID == userId
                                select new DBUpdateDto
                                {
                                    UserName = audit.UserName,
                                    Action = new
                                    {
                                        value = audit.HttpMethod,
                                        color = (audit.HttpMethod == "POST" ? "#06ab12" : audit.HttpMethod == "PUT" ? "#1792da" : "#da1717")
                                    },
                                    Entity = "project " + project.Name,

                                    ExcutitonTime = audit.ExecutionTime,
                                }
                               );

                issues.Concat(projects);
                var result = issues.OrderByDescending(i => i.ExcutitonTime).ToList().Distinct();
                var groupByDate = result.GroupBy(x => x.ExcutitonTime.ToString("yyyy-MM-dd"));
                returnData = groupByDate.Select(x =>
               {
                   var data = new ReturnUpdatesDto
                   {
                       Date = x.Key,
                       Data = x
                   };
                   return data;
               }).Skip(skipCount).Take(pageSize);
            }
            return returnData;
        }
        // test PagedResultDto
        public async Task<CustomPagedResultDto<ReturnUpdatesDto>> GetListUpdate(GetListDto input,string userId)
        {
            if (input.Filter.IsNullOrWhiteSpace() || input.Filter == "null")
            {
                input.Filter = "";
            }
            HistoryCompare historyCompare = new HistoryCompare();
            var projectQuery = await _projectRepository.Where(x => x.Name.Contains(input.Filter)).IgnoreQueryFilters().ToListAsync();
            var memberQuery = await _memberRepository.IgnoreQueryFilters().ToListAsync();
            var auditLogQuery = await _auditLogRepository.IgnoreQueryFilters().ToListAsync();
            var issueQuery = await _issueRepository.IgnoreQueryFilters().ToListAsync();
            var entityChangeQuery = await _enityChangeRepository.IgnoreQueryFilters().ToListAsync();
            var statusQuery = await _statusRepository.IgnoreQueryFilters().ToListAsync();
            var entityPropertyChangeQuery = await _entityPropertyChangeRepository.IgnoreQueryFilters().ToListAsync();
            IEnumerable<ReturnUpdatesDto> returnData;
            if (_userManager.CheckRoleByUserId(userId, "admin").Result)
            {
                var issues = (from audit in auditLogQuery
                              join entityChange in entityChangeQuery on audit.Id equals entityChange.AuditLogId
                              join issue in issueQuery on entityChange.EntityId equals issue.Id.ToString()
                              join member in memberQuery on issue.ProjectID equals member.ProjectID
                              join project in projectQuery on member.ProjectID equals project.Id
                              join entityPropertyChange in entityPropertyChangeQuery on entityChange.Id equals entityPropertyChange.EntityChangeId
                              where entityPropertyChange.PropertyName == "StatusID"
                              select new DBUpdateDto
                              {
                                  AuditLogId = audit.Id.ToString(),
                                  UserName = audit.UserName,
                                  Action = new
                                  {
                                      value = audit.HttpMethod,
                                      color = (audit.HttpMethod == "POST" ? "#06ab12" : audit.HttpMethod == "PUT" ? "#1792da" : "#da1717")
                                  },
                                  Entity = "issue " + issue.Name,
                                  InProject = project.Name,
                                  OldValue = statusQuery.Find(s => s.Id.ToString() == entityPropertyChange.OriginalValue?.Replace("\"", ""))?.Name,
                                  NewValue = statusQuery.Find(s => s.Id.ToString() == entityPropertyChange.NewValue?.Replace("\"", ""))?.Name,
                                  ExcutitonTime = audit.ExecutionTime
                              }
                         ).Distinct(historyCompare).ToList();
                var projects = (from audit in auditLogQuery
                                join entityChange in entityChangeQuery on audit.Id equals entityChange.AuditLogId
                                join member in memberQuery on entityChange.EntityId equals member.ProjectID.ToString()
                                join project in projectQuery on member.ProjectID equals project.Id
                                select new DBUpdateDto
                                {
                                    UserName = audit.UserName,
                                    Action = new
                                    {
                                        value = audit.HttpMethod,
                                        color = (audit.HttpMethod == "POST" ? "#06ab12" : audit.HttpMethod == "PUT" ? "#1792da" : "#da1717")
                                    },
                                    Entity = "project " + project.Name,

                                    ExcutitonTime = audit.ExecutionTime,
                                }
                               ).Distinct(historyCompare).ToList();

                issues.AddRange(projects);
                var result = issues.OrderByDescending(i => i.ExcutitonTime).ToList();
                var groupByDate = result.GroupBy(x => x.ExcutitonTime.ToString("yyyy-MM-dd"));
                returnData = groupByDate.Select(x =>
                {
                    var data = new ReturnUpdatesDto
                    {
                        Date = x.Key,
                        Data = x
                    };
                    return data;
                });
            }
            else
            {
                var issues = (from audit in auditLogQuery
                              join entityChange in entityChangeQuery on audit.Id equals entityChange.AuditLogId
                              join issue in issueQuery on entityChange.EntityId equals issue.Id.ToString()
                              join member in memberQuery on issue.ProjectID equals member.ProjectID
                              join project in projectQuery on member.ProjectID equals project.Id
                              join entityPropertyChange in entityPropertyChangeQuery on entityChange.Id equals entityPropertyChange.EntityChangeId
                              where member.UserID == userId && entityPropertyChange.PropertyName == "StatusID"
                              select new DBUpdateDto
                              {
                                  AuditLogId = audit.Id.ToString(),
                                  UserName = audit.UserName,
                                  Action = new
                                  {
                                      value = audit.HttpMethod,
                                      color = (audit.HttpMethod == "POST" ? "#06ab12" : audit.HttpMethod == "PUT" ? "#1792da" : "#da1717")
                                  },
                                  Entity = "issue " + issue.Name,
                                  InProject = project.Name,
                                  OldValue = statusQuery.Find(s => s.Id.ToString() == entityPropertyChange.OriginalValue?.Replace("\"", ""))?.Name,
                                  NewValue = statusQuery.Find(s => s.Id.ToString() == entityPropertyChange.NewValue?.Replace("\"", ""))?.Name,
                                  ExcutitonTime = audit.ExecutionTime
                              }
                         ).Distinct(historyCompare).ToList();
                var projects = (from audit in auditLogQuery
                                join entityChange in entityChangeQuery on audit.Id equals entityChange.AuditLogId
                                join member in memberQuery on entityChange.EntityId equals member.ProjectID.ToString()
                                join project in projectQuery on member.ProjectID equals project.Id
                                where member.UserID == userId
                                select new DBUpdateDto
                                {
                                    UserName = audit.UserName,
                                    Action = new
                                    {
                                        value = audit.HttpMethod,
                                        color = (audit.HttpMethod == "POST" ? "#06ab12" : audit.HttpMethod == "PUT" ? "#1792da" : "#da1717")
                                    },
                                    Entity = "project " + project.Name,

                                    ExcutitonTime = audit.ExecutionTime,
                                }
                               ).Distinct(historyCompare).ToList();

                issues.AddRange(projects);
                var result = issues.OrderByDescending(i => i.ExcutitonTime).ToList().Distinct();
                var groupByDate = result.GroupBy(x => x.ExcutitonTime.ToString("yyyy-MM-dd"));
                returnData = groupByDate.Select(x =>
                {
                    var data = new ReturnUpdatesDto
                    {
                        Date = x.Key,
                        Data = x
                    };
                    return data;
                });
            }
            var totalCount = returnData.Count();
            returnData = returnData.Skip(input.SkipCount).Take(input.MaxResultCount);
            var result1 = returnData.AsEnumerable();
            return new CustomPagedResultDto<ReturnUpdatesDto>
            {
                totalCount = totalCount,
                iEnumerable = result1
            };
            // return returnData;
        }
        public class CustomPagedResultDto<T>
        {
            public long totalCount { get; set; }
            public IEnumerable<T> iEnumerable { get; set; }
        }
        public async Task<List<ProjectDto>> GetListProject(GetListDto input)
        {
            if (input.Filter.IsNullOrWhiteSpace() || input.Filter == "null")
            {
                input.Filter = "";
            }

            var query = from project in _projectRepository
                        where project.Name.Contains(input.Filter)
                        select new { project };
            var queryResult = await AsyncExecuter.ToListAsync(query);
            
            var ProjectDtos = queryResult.Select(result=> {
                var ProjectDto = ObjectMapper.Map<Project, ProjectDto>(result.project);
                ProjectDto.UserId = result.project.CreatorId;
                ProjectDto.userName = _userRepository.GetAsync(x=>x.Id == result.project.CreatorId).Result.Name;
                ProjectDto.CountMember = _memberRepository.Count(x => x.ProjectID == result.project.Id);
                ProjectDto.countIssueClose = _issueRepository.Count(x => x.ProjectID == result.project.Id && x.FinishDate != null);
                ProjectDto.countIssue = _issueRepository.Count(x => x.ProjectID == result.project.Id);
                return ProjectDto;
            }).ToList();
            return ProjectDtos;
        }      
        public PagedResultDto<Users.UserDto> GetListUserAll(GetListDto input)
        {
            if (input.Filter.IsNullOrWhiteSpace() || input.Filter == "null")
            {
                input.Filter = "";
            }
            // IQueryable<AppUser> userQueryable = await _userRepository.GetQueryableAsync();
            var currentUserId = _tmtCurrentUser.Id;
            var query = from user in _userRepository
                        where user.Name.Contains(input.Filter.Trim()) && user.Id != currentUserId
                        orderby user.CreationTime descending
                        select new Users.UserDto()
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            CreationTime = user.CreationTime
                        };
            int totalCount = query.Count();
          //  query = query.Skip(input.SkipCount).Take(input.MaxResultCount);
            var queryResult = query.ToList();
            
            
            return new PagedResultDto<Users.UserDto>(
                totalCount,
                queryResult
            );
        }
        //public Task<IEnumerable<ReturnUpdatesDto>> GetUpdates()
        //{
        //    throw new NotImplementedException();
        //}
    }
}

        //public async Task<List<GetIssueByUserDto>> GetAllIssueByCurrentUser(string param)
        //{
        //    var currentUserId = _tmtCurrentUser.Id;
        //    var issueAssignedQuery = await _assignRepository.ToListAsync();
        //    var issueQuery = await _issueRepository.ToListAsync();
        //    var statusQuery = await _statusRepository.ToListAsync();
        //    var projectQuery = await _projectRepository.ToListAsync();

        //    Console.WriteLine(param);
        //    if (param == "all")
        //    {
        //        var query = (from issueAssigned in issueAssignedQuery
        //                     join issue in issueQuery on issueAssigned.IssueID equals issue.Id
        //                     join project in projectQuery on issue.ProjectID equals project.Id
        //                     join status in statusQuery on issue.StatusID equals status.Id
        //                     where issueAssigned.UserID == currentUserId
        //                     select new GetIssueByUserDto
        //                     {
        //                         Name = issue.Name,
        //                         ProjectName = project.Name,
        //                         StatusName = status.Name,
        //                         DueDate = issue.DueDate
        //                     }).ToList();

        //        return query;
        //    }
        //    else
        //    {
        //        var query = (from issueAssigned in issueAssignedQuery
        //                     join issue in issueQuery on issueAssigned.IssueID equals issue.Id
        //                     join project in projectQuery on issue.ProjectID equals project.Id
        //                     join status in statusQuery on issue.StatusID equals status.Id
        //                     where issue.CreatorId != issueAssigned.UserID && issueAssigned.UserID == currentUserId
        //                     select new GetIssueByUserDto
        //                     {
        //                         Name = issue.Name,
        //                         ProjectName = project.Name,
        //                         StatusName = status.Name,
        //                         DueDate = issue.DueDate
        //                     }).ToList();
        //        return query;
        //    }
        //}
        //    public async Task<IEnumerable<ReturnChartDto>> GetChart(string type)
        //    {
        //        var statusQuery = await _statusRepository.ToListAsync();
        //        var projectQuery = await _projectRepository.ToListAsync();
        //        var memberQuery = await _memberRepository.ToListAsync();
        //        var issueAssignedQuery = await _assignRepository.ToListAsync();
        //        var issueQuery = await _issueRepository.ToListAsync();
        //        var currentUser = _tmtCurrentUser;

        //        if (type == "issue")
        //        {
        //            IEnumerable<ReturnChartDto> returnData;
        //            if (_userManager.CheckRoleByUserId(currentUser.Id, "admin").Result)
        //            {

        //                var issues = from issueAssigned in issueAssignedQuery
        //                             join issue in issueQuery on issueAssigned.IssueID equals issue.Id
        //                             join project in projectQuery on issue.ProjectID equals project.Id
        //                             join status in statusQuery on issue.StatusID equals status.Id
        //                             select new ChartIssueDto
        //                             {
        //                                 StatusName = issue.FinishDate != null ? "Completed" : "Uncompleted",
        //                                 CreationTime = issue.CreationTime
        //                             };
        //                var groupByDate = issues.GroupBy(x => x.CreationTime.ToString("yyyy"));
        //                returnData = groupByDate.Select(x =>
        //                {
        //                    var data = new ReturnChartDto
        //                    {
        //                        Year = x.Key,
        //                        Data = x.OrderBy(x => x.CreationTime).GroupBy(e => e.CreationTime.ToString("MM"), (key, group) => new
        //                        {
        //                            Month = key,
        //                            Data = group.GroupBy(e => e.StatusName, (key, group) => new
        //                            {
        //                                StatusName = key,
        //                                Count = group.Count()
        //                            })
        //                        })
        //                    };
        //                    return data;
        //                });
        //            }
        //            else
        //            {

        //                var issues = from issueAssigned in issueAssignedQuery
        //                             join issue in issueQuery on issueAssigned.IssueID equals issue.Id
        //                             join project in projectQuery on issue.ProjectID equals project.Id
        //                             join status in statusQuery on issue.StatusID equals status.Id
        //                             where issueAssigned.UserID == currentUser.Id
        //                             select new ChartIssueDto
        //                             {
        //                                 StatusName = issue.FinishDate != null ? "Completed" : "Uncompleted",
        //                                 CreationTime = issue.CreationTime
        //                             };
        //                var groupByDate = issues.GroupBy(x => x.CreationTime.ToString("yyyy"));
        //                returnData = groupByDate.Select(x =>
        //                {
        //                    var data = new ReturnChartDto
        //                    {
        //                        Year = x.Key,
        //                        Data = x.OrderBy(x => x.CreationTime).GroupBy(e => e.CreationTime.ToString("MM"), (key, group) => new
        //                        {
        //                            Month = key,
        //                            Data = group.GroupBy(e => e.StatusName, (key, group) => new
        //                            {
        //                                StatusName = key,
        //                                Count = group.Count()
        //                            })
        //                        })
        //                    };
        //                    return data;
        //                });
        //            }
        //            return returnData;
        //        }
        //        else
        //        {
        //            IEnumerable<ReturnChartDto> returnData;
        //            if (_userManager.CheckRoleByUserId(currentUser.Id, "admin").Result)
        //            {
        //                var projects = from project in memberQuery
        //                               select project;
        //                var groupByDate = projects.GroupBy(x => x.CreationTime.ToString("yyyy"));
        //                 returnData = groupByDate.Select(x =>
        //                {
        //                    var data = new ReturnChartDto
        //                    {
        //                        Year = x.Key,
        //                        Data = x.OrderBy(x => x.CreationTime).GroupBy(e => e.CreationTime.ToString("MM"), (key, group) => new
        //                        {
        //                            Month = key,
        //                            Count = group.Count()
        //                        })
        //                    };
        //                    return data;
        //                });
        //            }
        //            else
        //            {
        //                var projects = from project in memberQuery
        //                               where project.UserID == currentUser.Id
        //                               select project;
        //                var groupByDate = projects.GroupBy(x => x.CreationTime.ToString("yyyy"));
        //                 returnData = groupByDate.Select(x =>
        //                {
        //                    var data = new ReturnChartDto
        //                    {
        //                        Year = x.Key,
        //                        Data = x.OrderBy(x => x.CreationTime).GroupBy(e => e.CreationTime.ToString("MM"), (key, group) => new
        //                        {
        //                            Month = key,
        //                            Count = group.Count()
        //                        })
        //                    };
        //                    return data;
        //                });
        //            }

        //            return returnData;
        //        }
        //    }
        //}
   //}