using BugTracking.Assignees;
using BugTracking.Attachments;
using BugTracking.Azure;
using BugTracking.Azures;
using BugTracking.Categories;
using BugTracking.Comments;
using BugTracking.DetailAttachments;
using BugTracking.Follows;
using BugTracking.Hub;
using BugTracking.IShareDto;
using BugTracking.LevelEnum;
using BugTracking.Members;
using BugTracking.Notifications;
using BugTracking.PriorityEnum;
using BugTracking.Projects;
using BugTracking.SendMail;
using BugTracking.Statuss;
using BugTracking.UserInforTFS;
using BugTracking.Users;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Issues
{
    [Authorize("BugTracking.Users")]
    public class IssueAppService : BugTrackingAppService, IIssueAppService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIssueRepository _issueRepository;
        private readonly IAzureRepository _azureRepository;
        private readonly IssueManager _issueManager;
        private readonly IRepository<AppUser, string> _userRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAssigneeRepository _assignRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IDetailAttachmentRepository _detailAttachmentRepository;
        private readonly AttachmentAppService _attachmentAppService;
        private readonly AssigneeService _assigneeService;
        private readonly FollowService _followService;
        private readonly UserManager _userManager;
        private readonly SendNotifyMailAppService _sendNotifyMailAppService;
        private readonly NotificationsAppService _notificationsAppService;
        private IHubContext<SignalR> _hub;
        private readonly IFollowRepository _followRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IUserInforTfsRepository _userInforTfsRepository;
        private readonly StatusService _statusService;
        public IssueAppService(
            IIssueRepository issueRepository,
            IssueManager issueManager,
            IRepository<AppUser, string> userRepository,
            IStatusRepository statusRepository,
            IProjectRepository projectRepository,
            ICategoryRepository categoryRepository,
            IHttpContextAccessor httpContextAccessor,
            IAssigneeRepository assignRepository,
            ICommentRepository commentRepository,
            ITMTCurrentUser tmtCurrentUser,
            IAttachmentRepository attachmentRepository,
            IDetailAttachmentRepository detailAttachmentRepository,
            UserManager userManager,
            AttachmentAppService attachmentAppService,
            AssigneeService assigneeService,
            SendNotifyMailAppService sendNotifyMailAppService,
           IHubContext<SignalR> hub,
           NotificationsAppService notificationsAppService,
           FollowService followService,
           IMemberRepository memberRepository,
           IFollowRepository followRepository,
           IBackgroundJobClient backgroundJobClient,
           IAzureRepository azureRepository,
           IUserInforTfsRepository userInforTfsRepository,
           StatusService statusService)
        {
            _issueRepository = issueRepository;
            _issueManager = issueManager;
            _userRepository = userRepository;
            _statusRepository = statusRepository;
            _projectRepository = projectRepository;
            _categoryRepository = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
            _assignRepository = assignRepository;
            _commentRepository = commentRepository;
            _tmtCurrentUser = tmtCurrentUser;
            _attachmentRepository = attachmentRepository;
            _detailAttachmentRepository = detailAttachmentRepository;
            _attachmentAppService = attachmentAppService;
            _userManager = userManager;
            _assigneeService = assigneeService;
            _sendNotifyMailAppService = sendNotifyMailAppService;
            _hub = hub;
            _notificationsAppService = notificationsAppService;
            _followService = followService;
            _followRepository = followRepository;
            _memberRepository = memberRepository;
            _backgroundJobClient = backgroundJobClient;
            _azureRepository = azureRepository;
            _userInforTfsRepository = userInforTfsRepository;
            _statusService = statusService;
        }

        public async Task<Guid> GetIdByName(Guid projectId, string name)
        {
            var issue = await _issueRepository.GetAsync(x => x.ProjectID == projectId && x.Name == name);
            return issue.Id;
        }

        public async Task<IssuesDto> GetByIdAsync(Guid Id)
        {
            var queryable = await _issueRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from issue in queryable
                        join user in _userRepository on issue.CreatorId equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join status in _statusRepository on issue.StatusID equals status.Id
                        join project in _projectRepository on issue.ProjectID equals project.Id
                        join cat in _categoryRepository on issue.CategoryID equals cat.Id into termCategory
                        from c in termCategory.DefaultIfEmpty()
                        where issue.Id == Id
                        select new
                        {
                            issue,
                            NameUser = u == null ? null : u.Name,
                            status,
                            project,
                            catName = c == null ? null : c.Name
                        };
            //Execute the query and get the book with author
            var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);
            if (queryResult == null)
            {
                throw new EntityNotFoundException(typeof(Issue), Id);
            }

            var issueDto = ObjectMapper.Map<Issue, IssuesDto>(queryResult.issue);
            if (issueDto.IdParent != Guid.Empty)
            {
                var issueParent = await _issueRepository.GetAsync(x => x.Id == issueDto.IdParent);
                var issueMapDto = ObjectMapper.Map<Issue, IssuesDto>(issueParent);
                issueMapDto.UserName = issueMapDto.CreatorId != null ? _userRepository.FirstOrDefault(x => x.Id == issueMapDto.CreatorId).Name : "";
                issueMapDto.AssigneesList = new List<AssigneeDto> { };
                foreach (Assignee assignee in _assignRepository
                .Where(assign => assign.IssueID == issueParent.Id))
                {
                    var assignDto = ObjectMapper.Map<Assignee, AssigneeDto>(assignee);
                    var user = _userRepository.Where(u => u.Id == assignDto.UserID).FirstOrDefault();
                    assignDto.Name = user.Name;
                    issueMapDto.AssigneesList.Add(assignDto);
                }
                issueDto.IssueParentDto = issueMapDto;
            }
            issueDto.UserName = queryResult.NameUser;
            issueDto.StatusName = queryResult.status.Name;
            issueDto.NzColor = queryResult.status.NzColor;
            issueDto.ProjectName = queryResult.project.Name;
            issueDto.CategoryName = queryResult.catName;
            issueDto.PriorityValue = Enum.GetName(typeof(Priority), issueDto.Priority);
            issueDto.LevelValue = Enum.GetName(typeof(Level), issueDto.IssueLevel);
            issueDto.IssuesChildDto = _statusService.IssuesChildDtos(Id, "");
            issueDto.IsHaveParent = issueDto.IdParent != Guid.Empty;
            var countAtt = await _attachmentRepository.GetListAsync(x => x.TableID == Id);
            if (countAtt.Count > 0)
            {
                issueDto.AttachmentListImage = new List<AttachmentDto> { };
                issueDto.AttachmentListVideo = new List<AttachmentDto> { };
                foreach (var att in _attachmentRepository.Where(att => att.TableID == Id))
                {
                    foreach (var detailAttachment in _detailAttachmentRepository.Where(d => d.AttachmentID == att.Id))
                    {
                        var attDto = ObjectMapper.Map<Attachment, AttachmentDto>(att);
                        var detailAttachmentImage = _detailAttachmentRepository.Where(d => d.AttachmentID == att.Id).FirstOrDefault();
                        attDto.Name = detailAttachmentImage.FileName;
                        attDto.Type = detailAttachmentImage.Type;
                        if (detailAttachment.Type.Contains("image"))
                        {
                            issueDto.AttachmentListImage.Add(attDto);
                        }
                        if (detailAttachment.Type.Contains("video"))
                        {
                            issueDto.AttachmentListVideo.Add(attDto);
                        }
                    }
                }
            }

            return issueDto;
        }
        public async Task<PagedResultDto<IssuesDto>> GetListAsync(GetListDto input)
        {
            if (input.Filter.IsNullOrWhiteSpace())
            {
                input.Filter = "";
            }
            var queryable = await _issueRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from issue in queryable
                        join user in _userRepository on issue.CreatorId equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join status in _statusRepository on issue.StatusID equals status.Id
                        join project in _projectRepository on issue.ProjectID equals project.Id
                        join cat in _categoryRepository on issue.CategoryID equals cat.Id into termCategory
                        from c in termCategory.DefaultIfEmpty()
                        where issue.Name.Contains(input.Filter)
                        orderby issue.CreationTime descending
                        select new
                        {
                            issue,
                            NameUser = u == null ? null : u.Name,
                            status,
                            project,
                            catName = c == null ? null : c.Name
                        };
            var totalCount = input.Filter == null
                ? await _issueRepository.CountAsync()
                : await _issueRepository.CountAsync(
                    issue => issue.Name.Contains(input.Filter));
            if (input.MaxResultCount > 0 && input.MaxResultCount != 10)
            {
                query = query.Skip(input.SkipCount).Take(input.MaxResultCount);
            }
            else query = query.Skip(input.SkipCount).Take(totalCount);
            var queryResult = await AsyncExecuter.ToListAsync(query);

            var issueDtos = queryResult.Select(x =>
            {
                var issueDto = ObjectMapper.Map<Issue, IssuesDto>(x.issue);
                issueDto.UserName = x.NameUser;
                issueDto.StatusName = x.status.Name;
                issueDto.ProjectName = x.project.Name;
                issueDto.CategoryName = x.catName;
                return issueDto;
            }).ToList();

            return new PagedResultDto<IssuesDto>(
                totalCount,
                issueDtos
            );
        }
        public List<string> GetCheckName(Guid ProjectId, string name)
        {
            var rs = new List<string> { };
            if (_issueRepository.Any(x => x.ProjectID == ProjectId && x.Name == name))
            {
                rs.Add("Name exist in this project! Try another name!");
            }
            return rs;
        }
        public async Task<IssuesDto> CreateIssueAttachment(CreateUpdateIssuesDto Entity)
        {
            if (Entity.Name.IsNullOrWhiteSpace())
            {
                throw new UserFriendlyException("Input Bug Name is not null!");
            }

            if (_issueRepository.Any(x => x.ProjectID == Entity.ProjectID && x.Name == Entity.Name))
            {
                throw new UserFriendlyException("Name exist in this project! Try another name!");
            }

            var statusOpen = await _statusRepository.GetAsync(x => x.Name == "Open");
            if(Entity.StatusID != Guid.Empty && Entity.StatusID != statusOpen.Id)
            {
                statusOpen = await _statusRepository.GetAsync(x => x.Id == Entity.StatusID);
            }
            var project = await _projectRepository.GetAsync(Entity.ProjectID);
            var CurrentUserId = _tmtCurrentUser.GetId();

            if(project.ProjectIdTFS != Guid.Empty)
            {
                if (Entity.Name.Length > 255)
                {
                    throw new UserFriendlyException("Max length of name is 255 char to sync to TFS! Try another name!");
                }
            }
            else
            {
                if (Entity.Name.Length > 500)
                {
                    throw new UserFriendlyException("Max length of name is 500 char! Try another name!");
                }
            }

            if (project.ProjectIdTFS != Guid.Empty && !_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
            {
                throw new UserFriendlyException("This project has been synchronized with Tfs, please update the information to continue!");
            }

            var maxIndex = _issueRepository.Where(x => x.StatusID == statusOpen.Id).Max(x => x.CurrentIndex);

            int CurrentIndex = maxIndex > 0
                ? maxIndex + 1
                : 0;

            var Today = DateTime.Now;
            var startDate = Entity.StartDate != null ? DateTime.Parse(Entity.StartDate.Value.Date.ToShortDateString() + " " + Today.ToLongTimeString()) : Entity.StartDate;

            var issue = await _issueManager.CreateAsync(
                Entity.Name,
                Entity.Description,
                Entity.Priority,
                Entity.IssueLevel,
                Entity.CategoryID,
                Entity.ProjectID,
                statusOpen.Id,
                Entity.DueDate,
                startDate,
                Entity.FinishDate,
                CurrentIndex,
                Entity.IdWit != 0 ? Entity.IdWit : 0,
                0,
                Entity.IdParent
            );

            var Issue = await _issueRepository.InsertAsync(issue);
            var IssueMap = ObjectMapper.Map<Issue, IssuesDto>(issue);
            var Creator = await _userRepository.GetAsync(x => x.Id == CurrentUserId);

            IssueMap.ProjectName = project.Name;
            IssueMap.UserName = Creator.Name;

            if (Entity.Attachments.Any())
            {
                await _attachmentAppService.CreateByListAttachmentAsync(Entity.Attachments, Issue.Id);
            }

            if (Entity.Assignees.Any())
            {
                await _assigneeService.CreateByListAsync(Entity.Assignees, Issue.Id, true);
            }

            if (Entity.NotifyMail.Any())
            {
                await _sendNotifyMailAppService.QueueAbpMailAsync(Entity.NotifyMail,
                    "You have new message from " + IssueMap.UserName,
                    "New issue from " + IssueMap.ProjectName + " , name: " +
                    Entity.Name + " Start Date: " + Entity.StartDate + " dueDate: " + Entity.DueDate);
            }

            if (_azureRepository.Any())
            {
                if (project.ProjectIdTFS != Guid.Empty)
                {
                    _backgroundJobClient.Enqueue<IAzureAppService>(x => x.CreateWIT(Issue.Id, IssueMap.ProjectName, issue.Name, issue.Description,
                        ((int)Enum.Parse(issue.Priority.GetType(), issue.Priority.ToString()) + 1).ToString(), Entity.Assignees.ToList(), CurrentUserId, Entity.IdParent));
                }
            }

            IssueMap.AssigneesList = _statusService.GetAssigneeDtos(Issue.Id, "");
            return IssueMap;
        }

        public async Task<IssuesDto> UpdateAsync(UpdateIssuesDto Entity, Guid Id)
        {
            try
            {
                if (Entity.Name.IsNullOrWhiteSpace())
                {
                    throw new UserFriendlyException("Input Bug Name is not null!");
                }
                var CurrentUserId = _tmtCurrentUser.GetId();
                var issue = await _issueRepository.GetAsync(Id);
                var project = await _projectRepository.GetAsync(Entity.ProjectID);
                if (project.ProjectIdTFS != Guid.Empty && !_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
                {
                    throw new UserFriendlyException("This project has been synchronized with Tfs, please update the information to continue!");
                }
                if (issue == null)

                {
                    throw new EntityNotFoundException(typeof(Issue), Id);
                }
                else
                {
                    issue.Name = Entity.Name;
                    issue.Description = Entity.Description;
                    issue.Priority = Entity.Priority;
                    issue.IssueLevel = Entity.IssueLevel;
                    issue.CategoryID = Entity.CategoryID;
                    issue.DueDate = Entity.DueDate;
                    issue.ProjectID = Entity.ProjectID;
                    var IssueMap = ObjectMapper.Map<Issue, IssuesDto>(issue);
                    var Project = await _projectRepository.GetAsync(x => x.Id == IssueMap.ProjectID);
                    var Creator = await _userRepository.GetAsync(x => x.Id == CurrentUserId);
                    IssueMap.ProjectName = Project.Name;
                    IssueMap.UserName = Creator.Name;
                    var Issue = await _issueRepository.UpdateAsync(issue);
                    var assignees = await _assignRepository.GetListAsync(x => x.IssueID == Issue.Id);
                    //var assigneesDeleted = assignees.Select(x => x.UserID).Except(Entity.Assignees);
                    //var aList = assigneesDeleted.ToList();
                    //for (var i = 0; i < aList.Count; i++)
                    //{
                    //    await _notificationsAppService.InsertAsync(Id, aList[i], _userRepository.FirstOrDefault(x => x.Id == CurrentUserId).Name +
                    //        " Deleted You From: " + Entity.Name);
                    //}
                    //await _assignRepository.DeleteManyAsync(assignees);
                    var follows = await _followRepository.GetListAsync(x => x.IssueID == Issue.Id);
                    var usersFollow = follows.Select(x => x.UserID).ToArray();
                    await _notificationsAppService.InsertByListAsync(usersFollow, Issue.Id, Issue.Name + " Has Been Updated By: " + IssueMap.UserName);

                    await _attachmentAppService.UpdateAsync(Entity.Attachments, issue.Id);

                    if (Entity.NotifyMail.Any())
                    {
                        await _sendNotifyMailAppService.QueueAbpMailAsync(Entity.NotifyMail,
                            "You have new message from " + IssueMap.UserName,
                            "New issue from " + IssueMap.ProjectName + " , name: " + Entity.Name + " dueDate: " + Entity.DueDate);
                    }
                    //if (Entity.Assignees.Any())
                    //{
                    //    await _assigneeService.CreateByListAsync(Entity.Assignees, Issue.Id, true);
                    //}
                    if (_azureRepository.Any())
                    {
                        if (project.ProjectIdTFS != Guid.Empty)
                        {
                            _backgroundJobClient.Enqueue<IAzureAppService>(x => x.UpdateWIT(issue.Id, Entity.Name, Entity.Description,
                                ((int)Enum.Parse(issue.Priority.GetType(), issue.Priority.ToString()) + 1).ToString(), Entity.Assignees.ToList(), CurrentUserId));
                        }
                    }
                    return IssueMap;
                }
            }
            catch
            {
                throw new UserFriendlyException("An error while try to update issue !!!");
            }
        }
        public async Task<IssuesDto> UpdateAssingeeAsync(string[] Assignees, Guid Id)
        {
            try
            {
                var CurrentUserId = _tmtCurrentUser.GetId();
                var issue = await _issueRepository.GetAsync(Id);
                var Project = await _projectRepository.GetAsync(x => x.Id == issue.ProjectID);
                if (Project.ProjectIdTFS != Guid.Empty && !_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
                {
                    throw new UserFriendlyException("This project has been synchronized with Tfs, please update the information to continue!");
                }
                if (issue == null)

                {
                    throw new EntityNotFoundException(typeof(Issue), Id);
                }
                else
                {
                    var IssueMap = ObjectMapper.Map<Issue, IssuesDto>(issue);
                    var Creator = await _userRepository.GetAsync(x => x.Id == CurrentUserId);
                    IssueMap.ProjectName = Project.Name;
                    IssueMap.UserName = Creator.Name;
                    var Issue = await _issueRepository.UpdateAsync(issue);
                    var assignees = await _assignRepository.GetListAsync(x => x.IssueID == Issue.Id);
                    var assigneesDeleted = assignees.Select(x => x.UserID).Except(Assignees);
                    var aList = assigneesDeleted.ToList();
                    for (var i = 0; i < aList.Count; i++)
                    {
                        await _notificationsAppService.InsertAsync(Id, aList[i], _userRepository.FirstOrDefault(x => x.Id == CurrentUserId).Name +
                            " Deleted You From: " + issue.Name);
                        var follow = _followRepository.GetAsync(x => x.UserID == aList[i] && x.IssueID == Id);
                        await _followService.DeleteAsync(follow.Result.Id);
                    }
                    await _assignRepository.DeleteManyAsync(assignees);
                    if (Assignees.Any())
                    {
                        await _assigneeService.CreateByListAsync(Assignees, Issue.Id, true);
                    }
                    if (_azureRepository.Any())
                    {
                        _backgroundJobClient.Enqueue<IAzureAppService>(x => x.UpdateAssignee(Issue.Id, Assignees.ToList(), false, CurrentUserId));
                    }
                    return IssueMap;
                }
            }
            catch
            {
                throw new UserFriendlyException("An error while try to update issue !!!");
            }
        }
        public async Task UpdateIssuesByStatusIdAsync(Guid StatusId, Guid Id)
        {
            var issue = await _issueRepository.GetAsync(Id);
            var oldStatus = await _statusRepository.GetAsync(issue.StatusID);
            var status = await _statusRepository.GetAsync(StatusId);
            var CurrentUserId = _tmtCurrentUser.GetId();
            var project = await _projectRepository.GetAsync(issue.ProjectID);
            if (project.ProjectIdTFS != Guid.Empty && !_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
            {
                throw new UserFriendlyException("This project has been synchronized with Tfs, please update the information to continue!");
            }
            if (issue == null)

            {
                throw new EntityNotFoundException(typeof(Issue), Id);
            }
            else
            {
                bool finishedCheck = issue.StatusID != _statusRepository.FirstOrDefault(x => x.Name == "Closed").Id;
                issue.StatusID = StatusId;
                if (!finishedCheck)
                {

                    issue.FinishDate = null;
                    await _issueRepository.UpdateAsync(issue);
                }
                else
                {
                    if (status.Name == "Closed")
                    {
                        issue.FinishDate = DateTime.Now;
                        await _issueRepository.UpdateAsync(issue);
                    }
                    else await _issueRepository.UpdateAsync(issue);
                }
                var assignees = await _assignRepository.GetListAsync(x => x.IssueID == Id);
                for (var i = 0; i < assignees.Count; i++)
                {
                    await _notificationsAppService.InsertAsync(Id, assignees[i].UserID, issue.Name + " Update Status: " + oldStatus.Name + " -> " + status.Name);
                }
                _backgroundJobClient.Enqueue<IAzureAppService>(x => x.UpdateState(issue.Id, status.Id, CurrentUserId));
            }
        }

        public async Task DeleteByIdAsync(Guid Id)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var issue = await _issueRepository.GetAsync(Id);
            var project = await _projectRepository.GetAsync(issue.ProjectID);
            if (project.ProjectIdTFS != Guid.Empty && !_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
            {
                throw new UserFriendlyException("This project has been synchronized with Tfs, please update the information to continue!");
            }
            if (issue == null)
            {
                throw new EntityNotFoundException(typeof(Issue), Id);
            }
            else
            {
                var assignees = await _assignRepository.GetListAsync(x => x.IssueID == Id);
                var comments = await _commentRepository.GetListAsync(x => x.IssueID == Id);
                var attachments = await _attachmentRepository.GetListAsync(x => x.TableID == Id);
                if (attachments.Any())
                {
                    foreach (Attachment attachment in attachments)
                    {
                        await _attachmentAppService.DeleteAsync(attachment.Id);
                    }
                }
                if (comments.Any())
                {
                    await _commentRepository.DeleteManyAsync(comments);
                }
                if (assignees.Any())
                {
                    await _assignRepository.DeleteManyAsync(assignees);
                }
                await _notificationsAppService.InsertByListAsync(Id,
                _userRepository.FindAsync(CurrentUserId).Result.Name + " Deleted: "
                + _issueRepository.FindAsync(Id).Result.Name);
                if (_azureRepository.Any())
                {
                    if (project.ProjectIdTFS != Guid.Empty)
                    {
                        _backgroundJobClient.Enqueue<IAzureAppService>(x => x.DeleteWIT(issue.IdWIT, issue.ProjectID, CurrentUserId));
                    }
                }
                await _issueRepository.DeleteAsync(issue);
            }
        }
        public async Task DeleteIssue(Guid Id)
        {
            await _issueRepository.DeleteAsync(Id);
        }
        public async Task<ListResultDto<CategoryLookupDto>> GetCategoryLookupAsync()
        {
            var cat = await _categoryRepository.GetListAsync();
            return new ListResultDto<CategoryLookupDto>(
                ObjectMapper.Map<List<Category>, List<CategoryLookupDto>>(cat));
        }

        public async Task<ListResultDto<ProjectLookupDto>> GetProjectLookupAsync()
        {
            var pro = await _projectRepository.GetListAsync();
            return new ListResultDto<ProjectLookupDto>(
                ObjectMapper.Map<List<Project>, List<ProjectLookupDto>>(pro));
        }

        public async Task<ListResultDto<UserLookupDto>> GetUserLookupAsync()
        {
            var user = await _userRepository.GetListAsync();
            return new ListResultDto<UserLookupDto>(
                ObjectMapper.Map<List<AppUser>, List<UserLookupDto>>(user));
        }

        public async Task<ListResultDto<StatusLookupDto>> GetStatusLookupAsync()
        {
            var status = await _statusRepository.GetListAsync();
            return new ListResultDto<StatusLookupDto>(
                ObjectMapper.Map<List<Status>, List<StatusLookupDto>>(status));
        }

        public async Task<List<IssuesNoParentDto>> GetListIssueNoParent(Guid projectId)
        {
            var issues = await _issueRepository.GetListAsync(x => x.ProjectID == projectId && x.IdParent == Guid.Empty);
            return new List<IssuesNoParentDto>(
                ObjectMapper.Map<List<Issue>, List<IssuesNoParentDto>>(issues));
        }

        public async Task<PagedResultDto<IssuesDto>> GetListIssueByIdProject(GetListDto input, Guid IdProject, string IdCategory, string idStatus, string createrId, string assigneeId)

        {
            var CurrentUserId = _tmtCurrentUser.GetId();

            if (IdCategory.IsNullOrWhiteSpace() || IdCategory == "null")
            {
                IdCategory = "";
            }

            if (input.Filter.IsNullOrWhiteSpace() || input.Filter == "null")
            {
                input.Filter = "";
            }
            if (idStatus.IsNullOrWhiteSpace() || idStatus == "null")
            {
                idStatus = "";
            }
            if (createrId.IsNullOrWhiteSpace() || createrId == "null")
            {
                createrId = "";
            }
            if (assigneeId.IsNullOrWhiteSpace() || assigneeId == "null")
            {
                assigneeId = "";
            }
            bool isProjectCreator = _projectRepository.Any(x => x.Id == IdProject && x.CreatorId == CurrentUserId);
            bool isAdmin = await _userManager.CheckRoleByUserId(CurrentUserId, "admin");
            var admin = from issue in _issueRepository
                        join user in _userRepository on issue.CreatorId equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join status in _statusRepository on issue.StatusID equals status.Id
                        join project in _projectRepository on issue.ProjectID equals project.Id
                        join cat in _categoryRepository on issue.CategoryID equals cat.Id into termCategory
                        from c in termCategory.DefaultIfEmpty()
                        where issue.ProjectID == IdProject && issue.CategoryID.ToString().Contains(IdCategory) && issue.Name.Contains(input.Filter.Replace(" ", ""))
                        && issue.StatusID.ToString().Contains(idStatus)
                            && issue.CreatorId.Contains(createrId)
                        orderby issue.IssueLevel descending
                        select new IssuesDto()
                        {
                            UserID = issue.CreatorId,
                            UserName = u == null ? null : u.Name,
                            StatusID = issue.StatusID,
                            StatusName = status == null ? null : status.Name,
                            ProjectID = issue.ProjectID,
                            ProjectName = project == null ? null : project.Name,
                            CategoryID = issue.CategoryID,
                            CategoryName = c == null ? null : c.Name,
                            CurrentIndex = issue.CurrentIndex,
                            Description = issue.Description,
                            DueDate = issue.DueDate,
                            FinishDate = issue.FinishDate,
                            Id = issue.Id,
                            IssueLevel = issue.IssueLevel,
                            Name = issue.Name,
                            Priority = issue.Priority,
                            StartDate = issue.StartDate,
                            NzColor = status.NzColor,
                            DueInDay = countDaysAgo(issue.DueDate, issue.CreationTime),
                            DueFull = countDay(issue.DueDate, issue.CreationTime),
                            IdWIT = project.ProjectIdTFS != Guid.Empty ? issue.IdWIT : 0,
                            IdParent = issue.IdParent

                        };
            var notadmin = from issue in _issueRepository
                           join user in _userRepository on issue.CreatorId equals user.Id into termUser
                           from u in termUser.DefaultIfEmpty()
                           join status in _statusRepository on issue.StatusID equals status.Id
                           join project in _projectRepository on issue.ProjectID equals project.Id
                           join cat in _categoryRepository on issue.CategoryID equals cat.Id into termCategory
                           from c in termCategory.DefaultIfEmpty()
                           join assignee in _assignRepository on issue.Id equals assignee.IssueID into termAssignee
                           from a in termAssignee.DefaultIfEmpty()
                           where issue.ProjectID == IdProject && issue.CategoryID.ToString().Contains(IdCategory) && issue.Name.Contains(input.Filter.Replace(" ", "")) && issue.StatusID.ToString().Contains(idStatus)
                               && issue.CreatorId.Contains(createrId) && (a.UserID == CurrentUserId || issue.CreatorId == CurrentUserId)
                           orderby issue.IssueLevel descending
                           select new IssuesDto()
                           {
                               UserID = issue.CreatorId,
                               UserName = u == null ? null : u.Name,
                               StatusID = issue.StatusID,
                               StatusName = status == null ? null : status.Name,
                               ProjectID = issue.ProjectID,
                               ProjectName = project == null ? null : project.Name,
                               CategoryID = issue.CategoryID,
                               CategoryName = c == null ? null : c.Name,
                               CurrentIndex = issue.CurrentIndex,
                               Description = issue.Description,
                               DueDate = issue.DueDate,
                               FinishDate = issue.FinishDate,
                               Id = issue.Id,
                               IssueLevel = issue.IssueLevel,
                               Name = issue.Name,
                               Priority = issue.Priority,
                               StartDate = issue.StartDate,
                               NzColor = status.NzColor,
                               DueInDay = countDaysAgo(issue.DueDate, issue.CreationTime),
                               DueFull = countDay(issue.DueDate, issue.CreationTime),
                               IdWIT = project.ProjectIdTFS != Guid.Empty ? issue.IdWIT : 0,
                               IdParent = issue.IdParent

                           };
            //var queryResult = admin;
            if(admin.Count() == 0 && notadmin.Count() == 0)
            {
                List<IssuesDto> result = new List<IssuesDto>();
                return new PagedResultDto<IssuesDto>(
                    0,
                    result
                );
            }
            else
            {
                var queryResult = (isAdmin || isProjectCreator) ? admin : notadmin;
                var totalCount = queryResult.Count();
                if (input.MaxResultCount == 999)
                {
                    queryResult = queryResult.Skip(input.SkipCount).Take(totalCount);
                }
                else
                {
                    queryResult = queryResult.Skip(input.SkipCount).Take(input.MaxResultCount);
                }
                var result = queryResult != null ? queryResult.ToList() : null;
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        item.PriorityValue = Enum.GetName(typeof(Priority), item.Priority);
                        item.LevelValue = Enum.GetName(typeof(Level), item.IssueLevel);
                    }
                }
                return new PagedResultDto<IssuesDto>(
                    totalCount,
                    result
                );
            }
        }


        public List<object> GetEnumPriorityValue()
        {
            List<object> list = new List<object> { };
            var listOfEnums = Enum.GetValues(typeof(Priority)).Cast<Priority>().ToList();
            _ = Enum.GetValues(typeof(Priority));
            foreach (int enumValue in listOfEnums)
            {
                list.Add(Enum.GetName(typeof(Priority), enumValue));
            }
            return list;
        }

        public List<object> GetEnumLevelValue()
        {
            List<object> list = new List<object> { };
            var listOfEnums = Enum.GetValues(typeof(Level)).Cast<Level>().ToList();
            _ = Enum.GetValues(typeof(Level));
            foreach (int enumValue in listOfEnums)
            {
                list.Add(Enum.GetName(typeof(Level), enumValue));
            }
            return list;
        }

        public async Task UpdateAllByList(UpdateBoardDto data, Guid IdIssue)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var projectId = _issueRepository.GetAsync(IdIssue).Result.ProjectID;
            var project = await _projectRepository.GetAsync(projectId);
            if (project.ProjectIdTFS != Guid.Empty && !_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
            {
                throw new UserFriendlyException("This project has been synchronized with Tfs, please update the information to continue!");
            }
            if (data.Container != null)
            {
                var status = await _statusRepository.GetAsync(data.Container.StatusId);
                var containerCount = data.Container.IssuesId.Count;
                for (int i = 0; i < containerCount; i++)
                {
                    var issue = await _issueRepository.GetAsync(data.Container.IssuesId[i]);
                    issue.StatusID = data.Container.StatusId;
                    issue.CurrentIndex = data.Index + i;
                    if (i == 0)
                    {
                        if (status.Name == "Closed")
                        {
                            issue.FinishDate = DateTime.Now;
                        }
                        else
                        {
                            issue.FinishDate = null;
                        }
                    }
                    await _issueRepository.UpdateAsync(issue);
                }
                if (data.PreviousContainer != null)
                {
                    var previousContainerCount = data.PreviousContainer.IssuesId.Count;
                    for (int i = 0; i < previousContainerCount; i++)
                    {
                        var issue = await _issueRepository.GetAsync(data.PreviousContainer.IssuesId[i]);
                        issue.CurrentIndex = data.PreviousIndex + i;
                        await _issueRepository.UpdateAsync(issue);
                    }
                }
                await _notificationsAppService.InsertByListAsync(data.Container.IssuesId[0],
                _userRepository.FindAsync(CurrentUserId).Result.Name + " Update Status: " + status.Name + " On "
                + _issueRepository.FindAsync(data.Container.IssuesId[0]).Result.Name);
                _backgroundJobClient.Enqueue<IAzureAppService>(x => x.UpdateState(IdIssue, status.Id, CurrentUserId));
            }
            else
            {
                var previousContainerCount = data.PreviousContainer.IssuesId.Count;
                for (int i = 0; i < previousContainerCount; i++)
                {
                    var issue = await _issueRepository.GetAsync(data.PreviousContainer.IssuesId[i]);
                    issue.CurrentIndex = data.PreviousIndex + i;
                    await _issueRepository.UpdateAsync(issue);
                }
            }
        }

        public async Task UpdateDueDate(Guid ID, DateTime? date)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var issue = await _issueRepository.GetAsync(ID);
            if (issue == null)
            {
                throw new EntityNotFoundException(typeof(Issue), ID);
            }
            if (date == null)
            {
                issue.DueDate = null;
            }
            else issue.DueDate = date;
            await _issueRepository.UpdateAsync(issue);
            await _notificationsAppService.InsertByListAsync(ID,
                _userRepository.FindAsync(CurrentUserId).Result.Name + " has update due Date to " + date +
                " on issue you're following: "
                + _issueRepository.FindAsync(ID).Result.Name);
        }

        public async Task UpdateFinishDate(Guid ID, DateTime date)
        {
            var issue = await _issueRepository.GetAsync(ID);
            if (issue == null)
            {
                throw new EntityNotFoundException(typeof(Issue), ID);
            }
            issue.FinishDate = date;
            await _issueRepository.UpdateAsync(issue);
        }

        public async Task<IssuesDto> UpdateCategory(Guid ID, Guid category)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var issue = await _issueRepository.GetAsync(ID);
            var categoryNew = await _categoryRepository.GetAsync(category);
            if (issue == null)
            {
                throw new EntityNotFoundException(typeof(Issue), ID);
            }
            issue.CategoryID = category;
            await _issueRepository.UpdateAsync(issue);
            await _notificationsAppService.InsertByListAsync(ID,
                _userRepository.FindAsync(CurrentUserId).Result.Name + " Updated Category: " + categoryNew.Name + " On "
                + _issueRepository.FindAsync(ID).Result.Name);
            return ObjectMapper.Map<Issue, IssuesDto>(issue);
        }

        public async Task<PagedResultDto<IssuesDto>> GetListCateByIdProject(Guid IdProject)
        {
            var queryable = await _issueRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from issue in queryable
                        join project in _projectRepository on issue.ProjectID equals project.Id
                        join cat in _categoryRepository on issue.CategoryID equals cat.Id
                        where issue.ProjectID == IdProject
                        select new
                        {
                            issue,
                            project,
                            cat
                        };
            var queryResult = await AsyncExecuter.ToListAsync(query);

            var issueDtos = queryResult.Select(x =>
            {
                var issueDto = ObjectMapper.Map<Issue, IssuesDto>(x.issue);
                issueDto.CategoryName = x.cat.Name;
                return issueDto;
            }).ToList();

            var totalCount = _issueRepository.Count(x => x.ProjectID == IdProject);
            return new PagedResultDto<IssuesDto>(
                totalCount,
                issueDtos
            );
        }

        public async Task<PagedResultDto<IssuesDto>> GetListIssueCreatedByMe(string Filter, string IdStatus, string Idproject, bool IsAss, int take, int skip)
        {
            var id = _tmtCurrentUser.GetId();
            if (Filter.IsNullOrWhiteSpace() || Filter == "null")
            {
                Filter = "";
            }
            if (IdStatus.IsNullOrWhiteSpace() || IdStatus == "null")
            {
                IdStatus = "";
            }
            if (Idproject.IsNullOrWhiteSpace() || Idproject == "null")
            {
                Idproject = "";
            }
            // issue assigned
            if (IsAss)
            {
                var query = from assign in _assignRepository
                            join issue in _issueRepository on assign.IssueID equals issue.Id into termIssue
                            from i in termIssue.DefaultIfEmpty()
                            join status in _statusRepository on i.StatusID equals status.Id
                            join project in _projectRepository on i.ProjectID equals project.Id
                            where assign.UserID == id && project.Id.ToString().Contains(Idproject)
                            && i.Name.Contains(Filter) && status.Id.ToString().Contains(IdStatus)
                            select new
                            {
                                i,
                                status,
                                project
                            };
                var queryResult = await AsyncExecuter.ToListAsync(query);

                var issueDtos = queryResult.Skip(skip).Take(take).Select(x =>
                {
                    var issueDto = ObjectMapper.Map<Issue, IssuesDto>(x.i);
                    issueDto.AssigneesList = new List<AssigneeDto> { };
                    issueDto.StatusName = x.status.Name;
                    issueDto.NzColor = x.status.NzColor;
                    issueDto.ProjectName = x.project.Name;
                    foreach (Assignee assignee in _assignRepository
                    .Where(assign => assign.IssueID == issueDto.Id))
                    {
                        var assignDto = ObjectMapper.Map<Assignee, AssigneeDto>(assignee);
                        var user = _userRepository.Where(u => u.Id == assignDto.UserID).FirstOrDefault();
                        assignDto.Name = user.Name;
                        issueDto.AssigneesList.Add(assignDto);
                    }
                    return issueDto;
                }).ToList();

                decimal totalCount = queryResult.Count;
                //if (issueDtos.Count > 0)
                //{
                //    decimal countClosed = issueDtos.Where(x => x.StatusName == "Closed").Count();
                //    totalCount = (Math.Round(countClosed / (decimal)issueDtos.Count, 2)) * 100;
                //}
                return new PagedResultDto<IssuesDto>(
                    (long)totalCount,
                    issueDtos
                );
            }
            // issue created by me
            else
            {
                var query = from issue in _issueRepository
                            join status in _statusRepository on issue.StatusID equals status.Id
                            join project in _projectRepository on issue.ProjectID equals project.Id
                            where issue.CreatorId == id && project.Id.ToString().Contains(Idproject)
                            && issue.Name.Contains(Filter) && status.Id.ToString().Contains(IdStatus)
                            select new
                            {
                                issue,
                                status,
                                project
                            };
                var queryResult = await AsyncExecuter.ToListAsync(query);

                var issueDtos = queryResult.Skip(skip).Take(take).Select(x =>
                {
                    var issueDto = ObjectMapper.Map<Issue, IssuesDto>(x.issue);
                    issueDto.AssigneesList = new List<AssigneeDto> { };
                    issueDto.StatusName = x.status.Name;
                    issueDto.NzColor = x.status.NzColor;
                    issueDto.ProjectName = x.project.Name;
                    foreach (Assignee assignee in _assignRepository
                    .Where(assign => assign.IssueID == issueDto.Id))
                    {
                        var assignDto = ObjectMapper.Map<Assignee, AssigneeDto>(assignee);
                        var user = _userRepository.Where(u => u.Id == assignDto.UserID).FirstOrDefault();
                        assignDto.Name = user.Name;
                        issueDto.AssigneesList.Add(assignDto);
                    }
                    return issueDto;
                }).ToList();

                decimal totalCount = queryResult.Count;
                //if (issueDtos.Count > 0)
                //{
                //    decimal countClosed = issueDtos.Where(x => x.StatusName == "Closed").Count();
                //    totalCount = (Math.Round(countClosed / (decimal)issueDtos.Count, 2)) * 100;
                //}
                return new PagedResultDto<IssuesDto>(
                    (long)totalCount,
                    issueDtos
                );
            }

        }

        public async Task<bool> GetCheckRoleAdmin(string idUser)
        {
            //var userId = _tmtCurrentUser.GetId();

            if (await _userManager.CheckRoleByUserId(idUser, "admin"))
            {
                return true;
            }
            else return false;
        }
        private static int countDaysAgo(DateTime? date1, DateTime date2)
        {
            if (date1 == null)
            {
                return 0;
            }
            else
            {
                var d1 = new DateTime(date1.Value.Year, date1.Value.Month, date1.Value.Day);
                var d2 = new DateTime(date2.Year, date2.Month, date2.Day);
                var d3 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                TimeSpan t = d1 - d2;
                TimeSpan t1 = d3 - d2;

                return (t.Days - t1.Days + 1);
            }

        }
        private static int countDay(DateTime? date1, DateTime date2)
        {
            if (date1 == null)
            {
                return 0;
            }
            else
            {
                var d1 = new DateTime(date1.Value.Year, date1.Value.Month, date1.Value.Day);
                var d2 = new DateTime(date2.Year, date2.Month, date2.Day);
                TimeSpan t = d1 - d2;

                return (t.Days + 1);
            }

        }
        public Guid GetProjectIdByIssue(Guid IssueId)
        {
            if (_issueRepository.Any(x => x.Id == IssueId))
            {
                return _issueRepository.GetAsync(IssueId).Result.ProjectID;
            }
            else
            {
                return Guid.Empty;
            }
        }
        public async Task UpdatePriority(Guid ID, Priority Priority)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var issue = await _issueRepository.GetAsync(ID);
            if (issue == null)
            {
                throw new EntityNotFoundException(typeof(Issue), ID);
            }
            issue.Priority = Priority;
            await _issueRepository.UpdateAsync(issue);
            await _notificationsAppService.InsertByListAsync(ID,
                _userRepository.FindAsync(CurrentUserId).Result.Name + " has update priority on issue you're following: "
                + _issueRepository.FindAsync(ID).Result.Name);
        }
        public async Task UpdateIssueLevel(Guid ID, Level IssueLevel)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var issue = await _issueRepository.GetAsync(ID);
            if (issue == null)
            {
                throw new EntityNotFoundException(typeof(Issue), ID);
            }
            issue.IssueLevel = IssueLevel;
            await _issueRepository.UpdateAsync(issue);
            await _notificationsAppService.InsertByListAsync(ID,
                _userRepository.FindAsync(CurrentUserId).Result.Name + " has update level on issue you're following: "
                + _issueRepository.FindAsync(ID).Result.Name);
        }
    }
}