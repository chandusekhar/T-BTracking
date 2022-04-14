using BugTracking.Assignees;
using BugTracking.Categories;
using BugTracking.Comments;
using BugTracking.Issues;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Users;
using BugTracking.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AuditLogging;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Follows
{
    [Authorize("BugTracking.Users")]
    public class FollowService : BugTrackingAppService, IFollowService
    {
        private readonly IFollowRepository _followRepository;
        private readonly FollowManager _followManager;
        private readonly IRepository<AppUser, string> _userRepository;
        private readonly IRepository<Issue, Guid> _isssueRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly IStatusRepository _statusRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IEnityChangeRepository _enityChangeRepository;
        private readonly IEnityPropertyRepository _entityPropertyChangeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IIssueChangedViewRepository _issueChangedViewRepository;
        private readonly IAssigneeRepository _assigneeRepository;
        public FollowService(
            IAssigneeRepository assigneeRepository,
            IIssueChangedViewRepository issueChangedViewRepository,
            ICommentRepository commentRepository,
            IEnityChangeRepository enityChangeRepository,
            IEnityPropertyRepository entityPropertyChangeRepository,
            IAuditLogRepository auditLogRepository,
            ICategoryRepository categoryRepository,
            IStatusRepository statusRepository,
            IFollowRepository followRepository, 
            FollowManager followManager, 
            IRepository<Issue, Guid> isssueRepository, 
            IRepository<Project, Guid> projectRepository,
            IRepository<AppUser, string> userRepository,
            ITMTCurrentUser tmtCurrentUser)
        {
            _assigneeRepository = assigneeRepository;
            _issueChangedViewRepository = issueChangedViewRepository;
            _commentRepository = commentRepository;
            _enityChangeRepository = enityChangeRepository;
            _entityPropertyChangeRepository = entityPropertyChangeRepository;
            _auditLogRepository = auditLogRepository;
            _categoryRepository = categoryRepository;
            _statusRepository = statusRepository;
            _followRepository = followRepository;
            _followManager = followManager;
            _isssueRepository = isssueRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _tmtCurrentUser = tmtCurrentUser;
        }
        public async Task<FollowDto> CreateAsync(CreateFollowDto input)
        {
            var ExistUserFollow = _followRepository.Any(x => x.IssueID == input.IssueID && x.UserID == input.UserID);
            if (ExistUserFollow)
            {
                throw new UserFriendlyException("User Already Exist !!");
            }
            var follow = _followManager.CreateAsync(
                input.IssueID,
                input.UserID
            );

            await _followRepository.InsertAsync(follow);

            return ObjectMapper.Map<Follow, FollowDto>(follow);
        }
        public async Task CreateByListAsync(string[] followList, Guid idIssue)
        {
            foreach (string f in followList)
            {
                if (!_followRepository.Any(x => x.UserID == f && x.IssueID == idIssue))
                {
                    var follow = _followManager.CreateAsync(
                        idIssue,
                        f
                    );
                    await _followRepository.InsertAsync(follow);
                }
            }
        }
        public async Task DeleteAsync(Guid id)
        {
            await _followRepository.DeleteAsync(id);
        }

        public async Task<FollowDto> GetByIdAsync(Guid id)
        {
            var query = from follow in _followRepository
                        join user in _userRepository on follow.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join issue in _isssueRepository on follow.IssueID equals issue.Id
                        join project in _projectRepository on issue.ProjectID equals project.Id
                        where follow.Id == id
                        select new { follow, NameUser = u == null ? null : u.Name, u, issue, project };

            var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);
            if (queryResult == null)
            {
                throw new EntityNotFoundException(typeof(Follow), id);
            }

            var memberDto = ObjectMapper.Map<Follow, FollowDto>(queryResult.follow);
            memberDto.UserName = queryResult.NameUser;
            memberDto.IssueName = queryResult.issue.Name;
            memberDto.UserID = queryResult.u.Id;
            memberDto.IssueID = queryResult.issue.Id;
            memberDto.ProjectId = queryResult.project.Id;
            memberDto.ProjectName = queryResult.project.Name;
            return memberDto;
        }
        public async Task<FollowDto> GetByUserIssueIdAsync(Guid issueId, string userId)
        {
            var queryable = await _followRepository.GetQueryableAsync();
            var query = from follow in queryable
                        join user in _userRepository on follow.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join issue in _isssueRepository on follow.IssueID equals issue.Id
                        join project in _projectRepository on issue.ProjectID equals project.Id
                        where follow.IssueID == issueId && follow.UserID == userId
                        select new { follow, NameUser = u == null ? null : u.Name, u, issue, project };
            var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);
            if (queryResult == null)
            {
                throw new UserFriendlyException("Not Exist !!");
            }

            var memberDto = ObjectMapper.Map<Follow, FollowDto>(queryResult.follow);
            memberDto.UserName = queryResult.NameUser;
            memberDto.IssueName = queryResult.issue.Name;
            memberDto.UserID = queryResult.u.Id;
            memberDto.IssueID = queryResult.issue.Id;
            memberDto.ProjectId = queryResult.project.Id;
            memberDto.ProjectName = queryResult.project.Name;
            return memberDto;
        }
        public PagedResultDto<FollowDto> GetListByIssueId(GetFollowDto input, Guid issueId)
        {
            if (input.Filter.IsNullOrWhiteSpace() || input.Filter == "null")
            {
                input.Filter = "";
            }

            var query = from follow in _followRepository
                        join user in _userRepository on follow.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join issue in _isssueRepository on follow.IssueID equals issue.Id into termIssue
                        from i in termIssue.DefaultIfEmpty()
                        join project in _projectRepository
                        on i.ProjectID equals project.Id into termProject
                        from p in termProject.DefaultIfEmpty()
                        where follow.IssueID == issueId
                        select new FollowDto()
                        {
                            ID = follow.Id,
                            IssueID = follow.IssueID,
                            IssueName = i == null ? null : i.Name,
                            UserID = follow.UserID,
                            UserName = u == null ? null : u.Name,
                            ProjectId = p.Id,
                            ProjectName = p == null ? null : p.Name,
                        };
            var totalCount = query.Count();
            query = query.Skip(input.SkipCount).Take(input.MaxResultCount);
            var result = query.ToList();
            return new PagedResultDto<FollowDto>(
                    totalCount,
                    result
                );
        }

        public PagedResultDto<FollowDto> GetListByUserId(GetFollowDto input, string userId)
        {
            if (input.Filter.IsNullOrWhiteSpace() || input.Filter == "null")
            {
                input.Filter = "";
            }

            var query = from follow in _followRepository
                        join user in _userRepository on follow.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join issue in _isssueRepository on follow.IssueID equals issue.Id into termIssue
                        from i in termIssue.DefaultIfEmpty()
                        join project in _projectRepository
                        on i.ProjectID equals project.Id into termProject
                        from p in termProject.DefaultIfEmpty()
                        where follow.UserID == userId
                        select new FollowDto()
                        {
                            ID = follow.Id,
                            IssueID = follow.IssueID,
                            IssueName = i == null ? null : i.Name,
                            UserID = follow.UserID,
                            UserName = u == null ? null : u.Name,
                            ProjectId = p.Id,
                            ProjectName = p == null ? null : p.Name,
                        };
            var totalCount = query.Count();
            query = query.Skip(input.SkipCount).Take(input.MaxResultCount);
            var result = query.ToList();
            return new PagedResultDto<FollowDto>(
                    totalCount,
                    result
                );
        }
        public PagedResultDto<FollowDto> GetListByUserIdProjectId(GetFollowDto input, Guid projectId)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            if (input.Filter.IsNullOrWhiteSpace() || input.Filter == "null")
            {
                input.Filter = "";
            }
            var query = from follow in _followRepository
                        join user in _userRepository on follow.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join issue in _isssueRepository on follow.IssueID equals issue.Id into termIssue
                        from i in termIssue.DefaultIfEmpty()
                        join project in _projectRepository
                        on i.ProjectID equals project.Id into termProject
                        from p in termProject.DefaultIfEmpty()
                        where follow.UserID == CurrentUserId && i.ProjectID == projectId
                        && i.Name.Contains(input.Filter) || u.Name.Contains(input.Filter)
                        select new FollowDto()
                        {
                            ID = follow.Id,
                            IssueID = follow.IssueID,
                            IssueName = i == null ? null : i.Name,
                            UserID = follow.UserID,
                            UserName = u == null ? null : u.Name,
                            ProjectId = p.Id,
                            ProjectName = p == null ? null : p.Name,
                        };
            var totalCount = query.Count();
            query = query.Skip(input.SkipCount).Take(input.MaxResultCount);
            var result = query.ToList();
            return new PagedResultDto<FollowDto>(
                    totalCount,
                    result
                );
        }
        public bool GetCheckFollow(Guid issueId)
        {
            var userId = _tmtCurrentUser.GetId();
            if (_followRepository.Any(x => x.UserID == userId && x.IssueID == issueId))
            {
                return true;
            }
            else return false;
        }

        public class IssuePropertiesWithListChangeDto
        {
            public string Id { get; set; }
            public string Name { get; set; }
            //public string Description { get; set; }
            public string StatusID { get; set; }
            public string StatusName { get; set; }
            public string StatusColor { get; set; }
            public string ProjectID { get; set; }
            public string ProjectName { get; set; }
            //public Priority Priority { get; set; }
            //public Guid? CategoryID { get; set; }
            public string DueDate { get; set; }
            //public DateTime? StartDate { get; set; }
            public string FinishDate { get; set; }
            public string LastChangeMethod { get; set; }
            public DateTime CreationTime { get; set; }
            public DateTime? LastModdificationTime { get; set; }

            public object ListChange { get; set; }
            public DateTime Excutiontime { get; set; }
        }
        public class IssueUpdateDto
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string LastChangeMethod { get; set; }
            public object ListChange { get; set; }
            public DateTime time { get; set; }
            public string UserName { get; set; }

        }
        public class ReturnUpdatesDto
        {
            public string Date { get; set; }
            public object Data { get; set; }
        }
        /////////////////////////////////////// CALENDAR ///////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        [AllowAnonymous]
        public async Task<ParallelQuery<ReturnUpdatesDto>> GetIssueUpdates(string issueId, int index, int pageSize, string filterName = "default", string filterValue = "default")
        {
            int skip = index * pageSize;
            var issueUpdatesQuery = await _issueChangedViewRepository.GetQueryableAsync();
            var issueUpdates = (from issueUpdate in issueUpdatesQuery.AsNoTracking()
                                where issueUpdate.IssueId == issueId
                                orderby issueUpdate.ExecutionTime descending
                                select new IssueUpdateDto
                                {
                                    Id = issueUpdate.IssueId,
                                    Name = issueUpdate.IssueName,
                                    LastChangeMethod = issueUpdate.HttpMethod,
                                    ListChange = issueUpdate.ListChange,
                                    time = issueUpdate.ExecutionTime,
                                    UserName = issueUpdate.UserName,
                                })
                                    .Skip(skip)
                                    .Take(pageSize);
            var results = await issueUpdates.ToListAsync();
            return results.AsParallel().AsOrdered().Select(e =>
            {
                e.ListChange = e.ListChange != null ? JsonSerializer.Deserialize<List<PropertiesChangeDto>>(e.ListChange.ToString()) : null;
                return e;
            }).GroupBy(x => x.time.ToString("yyyy-MM-dd")).Select(x =>
            {
                var data = new ReturnUpdatesDto
                {
                    Date = x.Key,
                    Data = x
                };
                return data;
            });
        }
        [AllowAnonymous]
        public async Task<ParallelQuery<ReturnUpdatesDto>> getTest(string projectId, int index, int pageSize, string filterName = "default", string filterValue = "default")
        {

            int skip = index * pageSize;
            var issueUpdatesQuery = await _issueChangedViewRepository.GetQueryableAsync();
            var issueUpdates = (from issueUpdate in issueUpdatesQuery.AsNoTracking()
                                join issue in _isssueRepository.AsNoTracking() on issueUpdate.IssueId equals issue.Id.ToString()
                                where issue.ProjectID.ToString() == projectId
                                orderby issueUpdate.ExecutionTime descending
                                select new IssuePropertiesWithListChangeDto
                                {
                                    Id = issueUpdate.IssueId,
                                    Name = issueUpdate.IssueName,
                                    StatusID = issueUpdate.IssueStatusID,
                                    StatusName = issueUpdate.IssueStatusName,
                                    StatusColor = issueUpdate.IssueStatusColor,
                                    ProjectID = issueUpdate.IssueProjectID,
                                    ProjectName = issueUpdate.IssueProjectName,
                                    DueDate = issueUpdate.IssueDueDate,
                                    LastChangeMethod = issueUpdate.HttpMethod,
                                    FinishDate = issueUpdate.IssueFinishDate,
                                    CreationTime = issue.CreationTime,
                                    LastModdificationTime = issue.LastModificationTime,
                                    ListChange = issueUpdate.ListChange,
                                    Excutiontime = issueUpdate.ExecutionTime
                                })
                                    .Skip(skip)
                                    .Take(pageSize);
            var results = await issueUpdates.ToListAsync();
            return results.AsParallel().AsOrdered().Select(e =>
            {
                e.ListChange = e.ListChange != null ? JsonSerializer.Deserialize<List<PropertiesChangeDto>>(e.ListChange.ToString()) : null;
                return e;
            }).GroupBy(x => x.Excutiontime.ToString("yyyy-MM-dd")).Select(x =>
            {
                var data = new ReturnUpdatesDto
                {
                    Date = x.Key,
                    Data = x
                };
                return data;
            });
        }
        public async Task<IEnumerable<IssuePropertiesWithListChangeDto>> GetListIssueFollow(int index, int pageSize, string filterName = "default", string filterValue = "default")
        {
            int skip = index * pageSize;
            string currentUserId = _tmtCurrentUser.Id;

            var issueQuery = await _isssueRepository.GetQueryableAsync(); //Without get deleted element
            var followIssueQuery = await _followRepository.GetQueryableAsync();
            var viewQuery = await _issueChangedViewRepository.GetQueryableAsync();

            var issues = (from issue in issueQuery.IgnoreQueryFilters().AsNoTracking()
                          join followIssue in followIssueQuery.IgnoreQueryFilters().AsNoTracking() on issue.Id equals followIssue.IssueID
                          where followIssue.UserID == currentUserId
                          orderby issue.LastModificationTime ?? issue.CreationTime descending
                          select issue)
                            .Skip(skip)
                            .Take(pageSize);
            //View has been attached issue last states, so i use it for performance
            var extendIssues = from issue in issues
                               join view in viewQuery.AsNoTracking() on issue.AuditLogId equals view.AuditLogId.ToString() into lj
                               from leftJoin in lj.DefaultIfEmpty()
                               orderby leftJoin.ExecutionTime descending
                                   //orderby issue.LastModificationTime ?? issue.CreationTime descending
                               select new IssuePropertiesWithListChangeDto
                               {
                                   Id = leftJoin.IssueId,
                                   Name = leftJoin.IssueName,
                                   StatusID = leftJoin.IssueStatusID,
                                   StatusName = leftJoin.IssueStatusName,
                                   StatusColor = leftJoin.IssueStatusColor,
                                   ProjectID = leftJoin.IssueProjectID,
                                   ProjectName = leftJoin.IssueProjectName,
                                   DueDate = leftJoin.IssueDueDate,
                                   FinishDate = leftJoin.IssueFinishDate,
                                   LastChangeMethod = leftJoin.HttpMethod,
                                   CreationTime = issue.CreationTime,
                                   LastModdificationTime = issue.LastModificationTime,
                                   ListChange = leftJoin.ListChange
                               };
            var extendIssuesList = await extendIssues.ToListAsync();
            return extendIssuesList.AsParallel().AsOrdered().Select(e =>
            {
                e.ListChange = e.ListChange != null ? JsonSerializer.Deserialize<List<PropertiesChangeDto>>(e.ListChange.ToString()) : null;
                return e;
            });
        }
    }
}
