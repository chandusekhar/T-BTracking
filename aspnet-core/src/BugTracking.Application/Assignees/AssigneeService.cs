using BugTracking.Attachments;
using BugTracking.Azures;
using BugTracking.DetailAttachments;
using BugTracking.Follows;
using BugTracking.Issues;
using BugTracking.LevelEnum;
using BugTracking.Notifications;
using BugTracking.PriorityEnum;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.UserInforTFS;
using BugTracking.Users;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Assignees
{
    [Authorize("BugTracking.Users")]
    public class AssigneeService : BugTrackingAppService, IAssigneeService
    {
        private readonly IAssigneeRepository _assigneeRepository;
        private readonly AssigneeManager _assigneeManager;
        private readonly IRepository<Issue, Guid> _isssueRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<AppUser, string> _userRepository;
        private readonly IRepository<Status, Guid> _statusRepository;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly UserManager _userManager;
        private readonly NotificationsAppService _notificationsAppService;
        private readonly FollowService _followService;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IDetailAttachmentRepository _detailAttRepository;
        private readonly IAzureRepository _azureRepository;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IUserInforTfsRepository _userInforTfsRepository;
        public AssigneeService(IAssigneeRepository assigneeRepository, 
            AssigneeManager assigneeManager, 
            IRepository<Issue, Guid> isssueRepository,
            IRepository<AppUser, string> userRepository, 
            IRepository<Project, Guid> projectRepository, 
            IRepository<Status, Guid> statusRepository, 
            ITMTCurrentUser tmtCurrentUser, 
            UserManager userManager, 
            NotificationsAppService notificationsAppService, 
            FollowService followService,
            IAttachmentRepository attachmentRepository, 
            IDetailAttachmentRepository detailAttRepository, 
            IAzureRepository azureRepository,
            IBackgroundJobClient backgroundJobClient, 
            IUserInforTfsRepository userInforTfsRepository)
        {
            _isssueRepository = isssueRepository;
            _assigneeRepository = assigneeRepository;
            _assigneeManager = assigneeManager;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _statusRepository = statusRepository;
            _tmtCurrentUser = tmtCurrentUser;
            _userManager = userManager;
            _notificationsAppService = notificationsAppService;
            _followService = followService;
            _attachmentRepository = attachmentRepository;
            _detailAttRepository = detailAttRepository;
            _azureRepository = azureRepository;
            _backgroundJobClient = backgroundJobClient;
            _userInforTfsRepository = userInforTfsRepository;
        }

        public AssigneeParentDto GetCheckIssueHaveParent(Guid issueId)
        {
            var isHaveParent = _isssueRepository.Any(x => x.Id == issueId && x.IdParent != Guid.Empty);

            return new AssigneeParentDto
            {
                IsHaveParent = isHaveParent,
                ParentId = isHaveParent ? _isssueRepository.FirstOrDefault(x => x.Id == issueId).IdParent : Guid.Empty
            };
        }
        public async Task<ListResultDto<UserAssigneesDto>> GetAssigneesByIssueId(Guid parentId, string issueId)
        {
            List<Assignee> assigneesIssues = new List<Assignee>();
            var assigneesParent = await _assigneeRepository.GetListAsync(x => x.IssueID == parentId);

            if(issueId != "null")
            {
                assigneesIssues = await _assigneeRepository.GetListAsync(x => x.IssueID == Guid.Parse(issueId));
            }
            
            var except = assigneesParent.Select(x=>x.UserID).Except(assigneesIssues.Select(x=>x.UserID)).ToList();

            List<UserAssigneesDto> userAssigneesDto = new List<UserAssigneesDto>();
            foreach(string userId in except)
            {
                var assignee = await _assigneeRepository.GetAsync(x => x.UserID == userId && x.IssueID == parentId);
                var assigneeMap = ObjectMapper.Map<Assignee, UserAssigneesDto>(assignee);
                assigneeMap.ID = _userRepository.FirstOrDefault(x => x.Id == assignee.UserID).Id;
                assigneeMap.Name = _userRepository.FirstOrDefault(x => x.Id == assignee.UserID).Name;
                userAssigneesDto.Add(assigneeMap);
            }
            return new ListResultDto<UserAssigneesDto>
            {
                Items = userAssigneesDto
            };

        }
        public async Task<AssigneeDto> CreateAsync(CreateDto input)
        {
            var checkExistUserInIssue = _assigneeRepository.Any(x => x.IssueID == input.IssueID && x.UserID == input.UserID);

            if (checkExistUserInIssue)
            {
                throw new UserFriendlyException("User Already Exist!!");
            }

            var assignee = _assigneeManager.CreateAsync(
                input.IssueID,
                input.UserID
            );

            await _assigneeRepository.InsertAsync(assignee);

            return ObjectMapper.Map<Assignee, AssigneeDto>(assignee);
        }
        public async Task CreateByListAsync(string[] assignList, Guid idIssue,bool IsDefault)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var assginees = await _assigneeRepository.GetListAsync(x => x.IssueID == idIssue);
            //var issue = await _isssueRepository.GetAsync(idIssue);
            //var project = await _projectRepository.GetAsync(issue.ProjectID);
            //if (project.ProjectIdTFS != Guid.Empty && !_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
            //{
            //    throw new UserFriendlyException("This project has been synchronized with Tfs, please update the information to continue!");
            //}
            var result = "";
            foreach (string assign in assignList)
            {
                //if (_assigneeRepository.Any(x => x.IssueID == idIssue && x.UserID == assign))
                //{
                //    continue;
                //}
                result = result + " - " + _userRepository.FirstOrDefault(x => x.Id == assign).Name;
                var assignee = _assigneeManager.CreateAsync(
                    idIssue,
                    assign
                );
                await _assigneeRepository.InsertAsync(assignee);
            }
            if (assginees.Any())
            {
                for (var i =0;i<assginees.Count;i++)
                {
                await _notificationsAppService.InsertAsync(idIssue,assginees[i].UserID,
                result + " Assigned To: "
                + _isssueRepository.FirstOrDefault(x => x.Id == idIssue).Name);
                }
            }
            await _followService.CreateByListAsync(assignList, idIssue);
            await _notificationsAppService.InsertByListAsync(assignList, idIssue,
                _userRepository.FirstOrDefault(x => x.Id == CurrentUserId).Name + " Assigned You Into Issue!");
            if (_azureRepository.Any())
            {
                _backgroundJobClient.Enqueue<IAzureAppService>(x=>x.UpdateAssignee(idIssue,assignList.ToList(), IsDefault, CurrentUserId));
            }
        }
        public async Task DeleteAsync(Guid id,Guid idIssue)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            if (_isssueRepository.Any(x => x.CreatorId == CurrentUserId && x.Id == idIssue) ||
                _assigneeRepository.Any(x => x.IssueID == idIssue && x.CreatorId == CurrentUserId) ||
                (await _userManager.CheckRoleByUserId(CurrentUserId, "admin")))
            {
            await _assigneeRepository.DeleteAsync(id);

            await _notificationsAppService.InsertByListAsync(idIssue,
                _userRepository.FindAsync(CurrentUserId).Result.Name + " Deleted "
                +_userRepository.FindAsync(_assigneeRepository.FindAsync(id).Result.UserID).Result.Name
                + "From  "
                + _isssueRepository.FindAsync(idIssue).Result.Name);
            }
            else
            {
                throw new UserFriendlyException("You're not allow to delete !!!");
            }
        }
        public async Task DeleteAssign(Guid id)
        {
            await _assigneeRepository.DeleteAsync(id);
        }
        public async Task<AssigneeDto> GetByIdAsync(Guid idIssue)
        {
            var query = from issue in _isssueRepository
                        join assignee in _assigneeRepository on issue.Id equals assignee.IssueID
                        join user in _userRepository on assignee.UserID equals user.Id
                        where assignee.IssueID == idIssue
                        select new { assignee, issue, user };

            var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);
            if (queryResult == null)
            {
                throw new EntityNotFoundException(typeof(Assignee), idIssue);
            }

            var assigneeDto = ObjectMapper.Map<Assignee, AssigneeDto>(queryResult.assignee);
            return assigneeDto;
        }
        public class IssueAssignee
        {
            public int CountIssue { get; set; }
            public int totalCount { get; set; }
            public PagedResultDto<AssigneeDto> pagedResultDto { get; set; }
        }
        public async Task<IssueAssignee> GetListAsyncByIdUser(string IdProject, string Filter, string CurrentUserId, int skip, int maxCount)
        {
            //var Today = DateTime.UtcNow;
            if (Filter.IsNullOrWhiteSpace() || Filter == "null")
            {
                Filter = "";
            }
            if (IdProject.IsNullOrWhiteSpace() || IdProject == "null")
            {
                IdProject = "";
            }

            var query = from issue in _isssueRepository
                        join project in _projectRepository on issue.ProjectID equals project.Id
                        join assignee in _assigneeRepository on issue.Id equals assignee.IssueID
                        join user in _userRepository on assignee.UserID equals user.Id
                        join status in _statusRepository on issue.StatusID equals status.Id
                        orderby assignee.CreationTime descending
                        where assignee.UserID == CurrentUserId && issue.ProjectID.ToString().Contains(IdProject)
                        && status.Id.ToString().Contains(Filter)
                        select new { assignee, issue, user, status, project };
            var totalIssueCount = query.Count();
            query = query
                .Skip(skip)
                .Take(maxCount);

            var queryResult = await AsyncExecuter.ToListAsync(query);
            var IssueNotClose = from issue in _isssueRepository
                                join assign in _assigneeRepository on issue.Id equals assign.IssueID
                                join status in _statusRepository on issue.StatusID equals status.Id
                                join project in _projectRepository on issue.ProjectID equals project.Id
                                where status.Name != "Closed" && project.Id.ToString().Contains(IdProject) && assign.UserID==CurrentUserId
                                select issue;
            var assigneeDtos = queryResult.Select(x =>
            {
                var assigneeDto = ObjectMapper.Map<Assignee, AssigneeDto>(x.assignee);
                var issueDto = ObjectMapper.Map<Issue, IssuesDto>(x.issue);
                assigneeDto.IssueName = x.issue.Name;
                assigneeDto.CreatorID = x.issue.CreatorId;
                assigneeDto.FinishDate = x.issue.FinishDate;
                assigneeDto.DueDate = x.issue.DueDate;
                assigneeDto.StatusName = x.status.Name;
                assigneeDto.IsClose = x.status.Name == "Closed";
                assigneeDto.NzColor = x.status.NzColor;
                assigneeDto.StartDate = x.issue.StartDate;
                assigneeDto.Description = x.issue.Description;
                assigneeDto.LastModificationTime = x.issue.LastModificationTime;
                assigneeDto.LastModifier = x.issue.LastModifierId!=null ? _userRepository.FirstOrDefault(u => u.Id == x.issue.LastModifierId).Name : null;
                assigneeDto.PriorityValue = Enum.GetName(typeof(Priority), issueDto.Priority);
                assigneeDto.LevelValue = Enum.GetName(typeof(Level), issueDto.IssueLevel);
                assigneeDto.AttachmentListImage = new List<AttachmentDto> { };
                assigneeDto.AttachmentListVideo = new List<AttachmentDto> { };
                assigneeDto.Project = x.project.Name;
                assigneeDto.ProjectID = x.project.Id;
                foreach (var att in _attachmentRepository.Where(att => att.TableID == assigneeDto.IssueID))
                {
                    foreach (var detailAttachment in _detailAttRepository.Where(d => d.AttachmentID == att.Id))
                    {
                        var attDto = ObjectMapper.Map<Attachment, AttachmentDto>(att);
                        var detailAttachmentImage = _detailAttRepository.FirstOrDefault(d => d.AttachmentID == att.Id);
                        attDto.Name = detailAttachmentImage.FileName;
                        if (detailAttachment.Type.Contains("image"))
                        {
                            assigneeDto.AttachmentListImage.Add(attDto);
                        }
                        if (detailAttachment.Type.Contains("video"))
                        {
                            assigneeDto.AttachmentListVideo.Add(attDto);
                        }
                    }
                }
                return assigneeDto;
            }).ToList();
            decimal totalCount = 0;
            if (assigneeDtos.Any())
            {
                decimal countClosed = assigneeDtos.Count(x => x.IsClose == true);
                totalCount = (Math.Round(countClosed / (decimal)totalIssueCount, 2)) * 100;
            }
            var result = new PagedResultDto<AssigneeDto>((long)totalCount, assigneeDtos);
            return new IssueAssignee{
                CountIssue= IssueNotClose.Count(),
                totalCount = totalIssueCount,
                pagedResultDto = result
        };
        }
        public int GetCountAssign(Guid ProjectId)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var query= from assign in _assigneeRepository.Where(x=>x.UserID == CurrentUserId)
                       join issue in _isssueRepository.Where(x=>x.ProjectID == ProjectId) on assign.IssueID equals issue.Id
                       join status in _statusRepository on issue.StatusID equals status.Id
                       where status.Name != "Closed"
                       select new {  issue };
            return query.Count();
        }
        public async Task<PagedResultDto<AssigneeDto>> GetListAsyncByIssueId(Guid IssueId)
        {
            var query = from issue in _isssueRepository
                        join project in _projectRepository on issue.ProjectID equals project.Id
                        join assignee in _assigneeRepository on issue.Id equals assignee.IssueID
                        join user in _userRepository on assignee.UserID equals user.Id
                        join status in _statusRepository on issue.StatusID equals status.Id
                        orderby issue.FinishDate, assignee.CreationTime descending
                        where issue.Id == IssueId
                        select new { assignee, issue, user, status };
            var maxCount = _assigneeRepository.Count(x => x.IssueID == IssueId);
            query = query
                .Take(maxCount);

            var queryResult = await AsyncExecuter.ToListAsync(query);

            var assigneeDtos = queryResult.Select(x =>
            {
                var assigneeDto = ObjectMapper.Map<Assignee, AssigneeDto>(x.assignee);
                assigneeDto.IssueName = x.issue.Name;
                assigneeDto.CreatorID = x.issue.CreatorId;
                assigneeDto.FinishDate = x.issue.FinishDate;
                assigneeDto.UserName = x.user.Name;
                var userTerm = _userRepository.Where(u => u.Id == assigneeDto.CreatorID).FirstOrDefault();
                assigneeDto.CreatorName =userTerm != null? userTerm.Name: "";
                assigneeDto.DueDate = x.issue.DueDate;
                assigneeDto.StatusName = x.status.Name;
                assigneeDto.IsClose = x.status.Name == "Closed";
                assigneeDto.NzColor = x.status.NzColor;
                return assigneeDto;
            }).ToList();
            var totalCount = query.Count();
            return new PagedResultDto<AssigneeDto>(totalCount, assigneeDtos);
        }

        public bool CheckUserHasBeenAssign(Guid IdProject)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var query = from issue in _isssueRepository
                        join project in _projectRepository on issue.ProjectID equals project.Id
                        join assignee in _assigneeRepository on issue.Id equals assignee.IssueID
                        where assignee.UserID == CurrentUserId && issue.ProjectID == IdProject
                        select new { assignee };
            return (query.Any());
        }

        public async Task UpdateAsync(Guid id, UpdateDto input)
        {
            var assignee = await _assigneeRepository.GetAsync(id);

            await _assigneeRepository.UpdateAsync(assignee);
        }
    }
}