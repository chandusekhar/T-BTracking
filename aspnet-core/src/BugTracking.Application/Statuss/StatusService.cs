using BugTracking.Assignees;
using BugTracking.Attachments;
using BugTracking.Categories;
using BugTracking.Comments;
using BugTracking.Follows;
using BugTracking.Issues;
using BugTracking.Projects;
using BugTracking.Users;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Statuss
{
    [RemoteService(true)]
    [Authorize("BugTracking.Users")]
    public class StatusService : BugTrackingAppService, IStatusService
    {
        private readonly IStatusRepository _statusRepository;
        private readonly IAssigneeRepository _assigneesRepository;
        private readonly StatusManager _statusManager;
        private readonly IRepository<Issue, Guid> _isssueRepository;
        private readonly IRepository<AppUser, string> _userRepository;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly UserManager _userManager;
        private readonly IProjectRepository _projectRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFollowRepository _followRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        public StatusService(
        IStatusRepository statusRepository,
        StatusManager statusManager,
        IRepository<Issue, Guid> isssueRepository,
        IAssigneeRepository assigneesRepository,
        IRepository<AppUser, string> userRepository,
        UserManager userManager,
        ITMTCurrentUser tMTCurrentUser,
        IProjectRepository projectRepository,
        ICategoryRepository categoryRepository,
        IFollowRepository followRepository,
        ICommentRepository commentRepository,
        IAttachmentRepository attachmentRepository)
        {
        _statusRepository = statusRepository;
        _statusManager = statusManager;
        _isssueRepository = isssueRepository;
        _assigneesRepository = assigneesRepository;
        _userRepository = userRepository;
        _tmtCurrentUser = tMTCurrentUser;
        _userManager = userManager;
        _projectRepository = projectRepository;
            _categoryRepository = categoryRepository;
            _followRepository = followRepository;
            _commentRepository = commentRepository;
            _attachmentRepository = attachmentRepository;
        }

        public async Task<StatusDTO> CreateAsync(CreateStatusDTO input)
        {
            var currentUserId = _tmtCurrentUser.GetId();
            if(!await _userManager.CheckRoleByUserId(currentUserId, "admin"))
            {
                throw new UserFriendlyException("You're not allow to add status !!!");
            }
            if (_statusRepository.Any(x => x.Name.ToLower().Trim() == input.Name.ToLower().Trim()))
            {
                throw new UserFriendlyException("Status already exist !!!");
            }
            if (input.NzColor == null)
            {
                throw new UserFriendlyException("Choose Color !!!");
            }
            int CurrentIndex = _statusRepository.Max(x => x.CurrentIndex) + 1;

            var status = await _statusManager.CreateAsync(
                input.Name.Trim(),
                CurrentIndex,
                false,
                string.Concat("#",input.NzColor)
            );
            await _statusRepository.InsertAsync(status);
            return ObjectMapper.Map<Status, StatusDTO>(status);
        }
        public async Task<List<StatusDTO>> GetListOnlyAsync()
        {
            var status = await _statusRepository.GetListAsync();

            return new List<StatusDTO>(
               ObjectMapper.Map<List<Status>, List<StatusDTO>>(status)
           );
        }
        public async Task DeleteAsync(Guid id)
        {
            var currentUserId = _tmtCurrentUser.GetId();
            if (!await _userManager.CheckRoleByUserId(currentUserId, "admin"))
            {
                throw new UserFriendlyException("You're not allow to delete status !!!");
            }
            var status = await _statusRepository.GetAsync(id);
            if (status.IsDefault)
            {
                throw new UserFriendlyException("Status default, cannot delete !!");
            }
            else
            {
                if (_isssueRepository.Any(x => x.StatusID == id))
                {
                    throw new UserFriendlyException("Status already exist issue, cannot delete !!!");
                }
                else
                {
                    await _statusRepository.DeleteAsync(id);
                }
            }
        }

        public async Task<StatusDTO> GetAsync(Guid id)
        {
            var status = await _statusRepository.GetAsync(id);
            return ObjectMapper.Map<Status, StatusDTO>(status);
        }
        public async Task<ListResultDto<StatusDTO>> GetListAsyncOnly()
        {
            var statuss = await _statusRepository.GetListAsync();

            return new ListResultDto<StatusDTO>(

               ObjectMapper.Map<List<Status>, List<StatusDTO>>(statuss)
           );
        }
        public async Task<List<StatusDTO>> GetListAsync()
        {
            var query = from status in _statusRepository
                        orderby status.CurrentIndex ascending
                        select new
                        { status };
            var queryResult = await AsyncExecuter.ToListAsync(query);
            var statusDtos = queryResult.Select(result =>
            {
                var statusDto = ObjectMapper.Map<Status, StatusDTO>(result.status);
                statusDto.CountIssue = _isssueRepository.Count(x => x.StatusID == result.status.Id);
                return statusDto;
            }).ToList();
            return statusDtos;
        }
        public async Task<bool> CheckIssueInProject(Guid idProject)
        {
            var result = await _isssueRepository.GetListAsync(x => x.ProjectID == idProject);
            return (result.Any());
        }
        public async Task CreateDefaultStatusAsync(string Name, int Index, string NzColor)
        {
           await _statusRepository.InsertAsync(await _statusManager.CreateAsync(Name, Index, true, NzColor));
        }
        public class IssuesResultChildDto
        {
            public int issuesClosed { get; set; }
            public List<IssuesDto> issuesDtos { get; set; }
        }
        public PagedResultDto<IssuesDto> IssuesChildDtos(Guid issueId, string idUser)
        {
            List<IssuesDto> issuesChildDtos = new List<IssuesDto> { };

            var issues = _isssueRepository.GetListAsync(x => x.IdParent == issueId);

            foreach(Issue issue in issues.Result)
            {
                var issueDto = ObjectMapper.Map<Issue, IssuesDto>(issue);

                issueDto.StatusName = _statusRepository.FindAsync(x => x.Id == issueDto.StatusID).Result.Name;
                issueDto.NzColor = _statusRepository.FindAsync(x => x.Id == issueDto.StatusID).Result.NzColor;
                var assignees = _assigneesRepository.GetListAsync(x => x.IssueID == issue.Id && x.UserID.Contains(idUser));

                issueDto.AssigneesList = new List<AssigneeDto> { };

                foreach (Assignee assignee in assignees.Result)
                {
                    var assignDto = ObjectMapper.Map<Assignee, AssigneeDto>(assignee);
                    assignDto.Name = _userRepository.FindAsync(x => x.Id == assignDto.UserID).Result.Name;

                    issueDto.AssigneesList.Add(assignDto);
                }

                issuesChildDtos.Add(issueDto);
            }

            return new PagedResultDto<IssuesDto>
            {
                TotalCount = issuesChildDtos.Count(x=>x.StatusName == "Closed"),
                Items = issuesChildDtos
            };
        }
        public async Task<List<StatusDTO>> GetListStatusAsync(Guid IdProject, string Filter, 
                                                                string idUser, string dueDate, string idCategory,int skip, int take)
        {
            var statusList = _statusRepository.OrderBy(x => x.CurrentIndex).ToList();
            var statusMap = ObjectMapper.Map<List<Status>, List<StatusDTO>>(statusList);

            foreach (StatusDTO statusDto in statusMap)
            {
                statusDto.IssueList = await GetIssuesByStatusAsync(IdProject, Filter, idUser, dueDate, idCategory, statusDto.Id, skip, take);
                statusDto.CountIssue = statusDto.IssueList.Count;
            };

            return statusMap;
        }
        public async Task<List<IssuesDto>> GetIssuesByStatusAsync(Guid IdProject, string Filter,
                                                                string idUser, string dueDate, string idCategory, Guid statusId,int skip, int take)
        {
            if (Filter.IsNullOrWhiteSpace() || Filter == "null")
            {
                Filter = "";
            }
            if (idUser.IsNullOrWhiteSpace() || idUser == "null")
            {
                idUser = "";
            }
            if (dueDate.IsNullOrWhiteSpace() || dueDate == "null")
            {
                dueDate = "";
            }
            if (idCategory.IsNullOrWhiteSpace() || idCategory == "null")
            {
                idCategory = "";
            }

            var currentUserId = _tmtCurrentUser.GetId();
            bool IsAdmin = await _userManager.CheckRoleByUserId(currentUserId, "admin") ? true : false;
            bool IsProjectCreator = _projectRepository.Any(x => x.Id == IdProject && x.CreatorId == currentUserId);

            var issues = _isssueRepository
            .Where(issue => issue.StatusID == statusId
            && issue.ProjectID == IdProject
            && issue.IdParent == Guid.Empty
            && issue.DueDate.ToString().Contains(dueDate)
            && issue.CategoryID.ToString().Contains(idCategory)
            && (IsAdmin || IsProjectCreator || (_assigneesRepository.Any(x => x.UserID == currentUserId && x.IssueID == issue.Id)) || issue.CreatorId == currentUserId))
            .Skip(skip).Take(take).OrderBy(x => x.CurrentIndex).ToList();

            var issuesMap = ObjectMapper.Map<List<Issue>, List<IssuesDto>>(issues);

            foreach (IssuesDto issueDto in issuesMap)
            {
                issueDto.IsHaveParent = issueDto.IdParent != Guid.Empty;

                issueDto.IssuesChildDto = IssuesChildDtos(issueDto.Id, idUser);

                issueDto.AssigneesList = GetAssigneeDtos(issueDto.Id, idUser);
            }

            if (!idUser.IsNullOrWhiteSpace())
            {
                issuesMap.RemoveAll(x => !x.AssigneesList.Any());
            }

            return issuesMap;
        }
        public async Task<List<CategoryBoardDto>> GetListCategoryAsync(Guid IdProject, string Filter,
                                                       string idUser, string dueDate, string idCategory)
        {
            var currentUserId = _tmtCurrentUser.GetId();
            bool IsAdmin = await _userManager.CheckRoleByUserId(currentUserId, "admin") ? true : false;
            bool isProjectCreator = _projectRepository.Any(x => x.Id == IdProject && x.CreatorId == currentUserId);
            if (Filter.IsNullOrWhiteSpace() || Filter == "null")
            {
                Filter = "";
            }
            if (idUser.IsNullOrWhiteSpace() || idUser == "null")
            {
                idUser = "";
            }
            if (dueDate.IsNullOrWhiteSpace() || dueDate == "null")
            {
                dueDate = "";
            }
            if (idCategory.IsNullOrWhiteSpace() || idCategory == "null")
            {
                idCategory = "";
            }
            var query = from category in _categoryRepository
                        select new { category };
            var queryResult = await AsyncExecuter.ToListAsync(query);

            var categoryDtos = queryResult.Select(result =>
            {
                var categoryDto = ObjectMapper.Map<Category, CategoryBoardDto>(result.category);
                categoryDto.IssueList = new List<IssuesDto> { };

                var issues = _isssueRepository
                .Where(issue => issue.CategoryID == result.category.Id && issue.ProjectID == IdProject
                && issue.Name.Replace(" ", "").Contains(Filter.Replace(" ", ""))
                && issue.DueDate.ToString().Contains(dueDate) && issue.StatusID.ToString().Contains(idCategory))
                .OrderBy(value => value.CurrentIndex);

                foreach (Issue issue in issues)
                {
                    var issueDto = ObjectMapper.Map<Issue, IssuesDto>(issue);
                    issueDto.AssigneesList = new List<AssigneeDto> { };

                    issueDto.IssuesChildDto = IssuesChildDtos(issue.Id, idUser);

                    foreach (Assignee assignee in _assigneesRepository
                    .Where(assign => assign.IssueID == issue.Id && assign.UserID.Contains(idUser)))
                    {
                        var assignDto = ObjectMapper.Map<Assignee, AssigneeDto>(assignee);
                        var user = _userRepository.FirstOrDefault(u => u.Id == assignDto.UserID);
                        assignDto.Name = user.Name;
                        issueDto.AssigneesList.Add(assignDto);
                    }
                    if (IsAdmin || isProjectCreator)
                    {
                        categoryDto.IssueList.Add(issueDto);
                    }
                    else
                    {
                        if (issueDto.CreatorId == currentUserId)
                        {
                            categoryDto.IssueList.Add(issueDto);
                        }
                        else
                        {
                            if (_assigneesRepository.Any(x => x.UserID == currentUserId && x.IssueID == issueDto.Id))
                            {
                                categoryDto.IssueList.Add(issueDto);
                            }
                        }
                    }
                    if (!idUser.IsNullOrWhiteSpace())
                    {
                        categoryDto.IssueList.RemoveAll(x => !x.AssigneesList.Any());
                    }
                }
                categoryDto.CountIssue = categoryDto.IssueList.Count;

                return categoryDto;
            }).ToList();

            return categoryDtos;
        }
        public async Task<IssuesDto> GetIssueDtoAsync(Guid issueId)
        {
            var issue = await _isssueRepository.GetAsync(issueId);
            var issueDto = ObjectMapper.Map<Issue, IssuesDto>(issue);
            issueDto.follows = _followRepository.Count(x => x.IssueID == issueDto.Id);
            issueDto.comments = _commentRepository.Count(x => x.IssueID == issueDto.Id);
            issueDto.attachments = _attachmentRepository.Count(x => x.TableID == issueDto.Id);
            issueDto.IsHaveParent = issueDto.IdParent != Guid.Empty;

            issueDto.IssuesChildDto = IssuesChildDtos(issueDto.Id, "");

            issueDto.AssigneesList = GetAssigneeDtos(issueDto.Id, "");

            return issueDto;
        }
        public List<AssigneeDto> GetAssigneeDtos(Guid issueId, string userId)
        {
            var assignees = _assigneesRepository
               .Where(assign => assign.IssueID == issueId
               && assign.UserID.Contains(userId)).ToList();

            var assigneeMap = ObjectMapper.Map<List<Assignee>, List<AssigneeDto>>(assignees);

            foreach (AssigneeDto assigneeDto in assigneeMap)
            {
                assigneeDto.Name = _userRepository.FirstOrDefault(x => x.Id == assigneeDto.UserID).Name;
            }

            return assigneeMap;
        }

        public async Task<Guid> GetIdbyName(string Name)
        {
            var result = await _statusRepository.FindAsync(x => x.Name == Name);
            return result.Id;
        }

        public async Task UpdateAsync(Guid id, UpdateStatusDTO input)
        {
            var StatusList =await _statusRepository.GetListAsync(x => x.Id != id);
            if (StatusList.Any(x=>x.Name.ToLower().Trim() == input.Name.ToLower().Trim()))
            {
               throw new UserFriendlyException("Status already exist !!!");
            }
            var status = await _statusRepository.GetAsync(id);
            if (status.IsDefault && status.Name != input.Name)
            {
                throw new UserFriendlyException("Status default, cannot update !!");
            }
            else
            {

                if (status.Name.Trim() != input.Name.Trim())
                {
                    await _statusManager.ChangeNameAsync(status, input.Name.Trim());
                }
                status.NzColor = input.NzColor;
                await _statusRepository.UpdateAsync(status);
            }

        }
        public async Task UpdateAllByList(List<StatusDTO> statusDtos)
        {
            for (int i = 0; i < statusDtos.Count; i++)
            {
                var status = await _statusRepository.GetAsync(statusDtos[i].Id);
                
                status.CurrentIndex = i;
                await _statusRepository.UpdateAsync(status);
            }
        }
        public async Task<PagedResultDto<StatusStatisticsDto>> GetStatusStatistics(Guid projectId)
        {
            List<StatusStatisticsDto> listResultDto = new List<StatusStatisticsDto>();
            var statuss = await _statusRepository.GetListAsync();
            foreach (var status in statuss)
            {
                var statusMap = ObjectMapper.Map<Status, StatusStatisticsDto>(status);
                statusMap.CountIssue = await _isssueRepository.CountAsync(x => x.StatusID == status.Id && x.ProjectID == projectId && x.IdParent == Guid.Empty);
                if (statusMap.CountIssue > 0)
                {
                    listResultDto.Add(statusMap);
                }
            }
            var totalCount = _isssueRepository.Count(x => x.ProjectID == projectId && x.IdParent == Guid.Empty);
            return new PagedResultDto<StatusStatisticsDto>
            {
                TotalCount = totalCount,
                Items = listResultDto
            };
        }


        //...SERVICE METHODS WILL COME HERE...
    }
}
