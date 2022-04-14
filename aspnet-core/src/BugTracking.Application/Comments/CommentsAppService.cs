using BugTracking.Assignees;
using BugTracking.Attachments;
using BugTracking.Azure;
using BugTracking.Azures;
using BugTracking.DetailAttachments;
using BugTracking.Follows;
using BugTracking.IShareDto;
using BugTracking.Issues;
using BugTracking.Members;
using BugTracking.Notifications;
using BugTracking.Projects;
using BugTracking.UserInforTFS;
using BugTracking.Users;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Comments
{
    [Authorize("BugTracking.Users")]
    public class CommentsAppService : BugTrackingAppService, ICommentService, IRepositoryCreateUpdateShareDto<CreateUpdateCommentDto, Guid>, IRepositoryDeleteGetByIdShare<CommentDto, Guid>
    {
        private readonly AttachmentManager _attachmentManager;
        private readonly ICommentRepository _commentRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly AttachmentAppService _attachmentService;
        private readonly CommentManager _commentManager;
        private readonly IRepository<Issue, Guid> _isssueRepository;
        private readonly IRepository<AppUser, string> _userRepository;
        private readonly IFollowRepository _followRepository;
        private readonly NotificationsAppService _notificationsAppService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IProjectRepository _projectRepository;
        private readonly IAzureRepository _azureRepository;
        private readonly AzureAppService _azureAppService;
        private readonly IDetailAttachmentRepository _detailAttachmentRepository;
        private readonly IUserInforTfsRepository _userInforTfsRepository;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly IMemberRepository _memberRepository;
        private readonly IAssigneeRepository _assigneeRepository;
        private readonly UserManager _userManager;
        public CommentsAppService(
            ICommentRepository commentRepository,
            CommentManager commentManager, IRepository<Issue, Guid> isssueRepository,
            IRepository<AppUser, string> userRepository, IFollowRepository followRepository,
            NotificationsAppService notificationsAppService,
            IBackgroundJobClient backgroundJobClient,
            IProjectRepository projectRepository,
            IAzureRepository azureRepository,
            AttachmentManager attachmentManager,
            IAttachmentRepository attachmentRepository,
            IDetailAttachmentRepository detailAttachmentRepository,
            AzureAppService azureAppService,
            AttachmentAppService attachmentService,
            UserManager userManager,
            IUserInforTfsRepository userInforTfsRepository,
            IMemberRepository memberRepository,
            IAssigneeRepository assigneeRepository,
            ITMTCurrentUser tmtCurrentUser)
        {
            _attachmentService = attachmentService;
            _assigneeRepository = assigneeRepository;
            _attachmentManager = attachmentManager;
            _attachmentRepository = attachmentRepository;
            _isssueRepository = isssueRepository;
            _commentRepository = commentRepository;
            _commentManager = commentManager;
            _userRepository = userRepository;
            _followRepository = followRepository;
            _notificationsAppService = notificationsAppService;
            _detailAttachmentRepository = detailAttachmentRepository;
            _backgroundJobClient = backgroundJobClient;
            _projectRepository = projectRepository;
            _azureRepository = azureRepository;
            _azureAppService = azureAppService;
            _userInforTfsRepository = userInforTfsRepository;
            _tmtCurrentUser = tmtCurrentUser;
            _memberRepository = memberRepository;
            _userManager = userManager;
        }

        public async Task DeleteByIdAsync(Guid Id)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var comment = await _commentRepository.GetAsync(Id);
            var attachments = await _attachmentRepository.GetListAsync(x => x.TableID == Id);
            var issue = await _isssueRepository.GetAsync(comment.IssueID);
            var project = await _projectRepository.GetAsync(issue.ProjectID);
            if (project.ProjectIdTFS != Guid.Empty && !_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
            {
                throw new UserFriendlyException("This project has been synchronized with Tfs, please update the information to continue!");
            }
            if (comment == null)

            {
                throw new EntityNotFoundException(typeof(Comment), Id);
            }
            else
            {

                if (attachments.Any())
                {
                    await _attachmentRepository.DeleteManyAsync(attachments);
                }
                if (_azureRepository.Any())
                {
                    _backgroundJobClient.Enqueue<IAzureAppService>(x => x.DeleteComment(comment.IssueID, project.Name, comment.WitCommentId, CurrentUserId));
                }
                await _commentRepository.DeleteAsync(comment);
            }
        }

        public async Task<CommentDto> GetByIdAsync(Guid id)
        {
            var query = from comment in _commentRepository
                        join user in _userRepository on comment.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join issue in _isssueRepository on comment.IssueID equals issue.Id into termIssue
                        from i in termIssue.DefaultIfEmpty()
                        where comment.Id == id
                        select new { comment, NameUser = u == null ? null : u.Name, NameIssue = i == null ? null : i.Name };

            var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);
            if (queryResult == null)
            {
                throw new EntityNotFoundException(typeof(Comment), id);
            }

            var commentDto = ObjectMapper.Map<Comment, CommentDto>(queryResult.comment);
            commentDto.UserName = queryResult.NameUser;
            commentDto.IssueName = queryResult.NameIssue;
            var countAtt = _attachmentRepository.GetListAsync(a => a.TableID == commentDto.Id);
            if (countAtt.Result.Any())
            {
                commentDto.AttachmentList = new List<AttachmentDto> { };
                foreach (var att in _attachmentRepository.Where(att => att.TableID == commentDto.Id))
                {
                    var attDto = ObjectMapper.Map<Attachment, AttachmentDto>(att);
                    commentDto.AttachmentList.Add(attDto);
                }
            }
            return commentDto;
        }
        public async Task<ListResultDto<CommentDto>> GetListAsync(GetListDto input)
        {
            var query = from comment in _commentRepository
                        join user in _userRepository on comment.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join issue in _isssueRepository on comment.IssueID equals issue.Id into termIssue
                        from i in termIssue.DefaultIfEmpty()
                        orderby input.Sorting
                        select new { comment, NameUser = u == null ? null : u.Name, NameIssue = i == null ? null : i.Name };

            var queryResult = await AsyncExecuter.ToListAsync(query);

            var commentDtos = queryResult.Select(x =>
            {
                var commentDto = ObjectMapper.Map<Comment, CommentDto>(x.comment);
                commentDto.UserName = x.NameUser;
                commentDto.IssueName = x.NameIssue;
                commentDto.datetime = x.comment.CreationTime;
                var countAtt = _attachmentRepository.GetListAsync(a => a.TableID == commentDto.Id);
                if (countAtt.Result.Any())
                {
                    commentDto.AttachmentList = new List<AttachmentDto> { };
                    foreach (var att in _attachmentRepository.Where(att => att.TableID == commentDto.Id))
                    {
                        var attDto = ObjectMapper.Map<Attachment, AttachmentDto>(att);
                        commentDto.AttachmentList.Add(attDto);
                    }
                }

                return commentDto;
            }).ToList();
            var totalCount = await _commentRepository.GetCountAsync();

            return new PagedResultDto<CommentDto>(
                totalCount,
                commentDtos
            );

        }
        public ListResultDto<CommentDto> GetListResultByIssueId(GetListDto input, Guid id)
        {
            if (input.Filter.IsNullOrWhiteSpace() || input.Filter == "null")
            {
                input.Filter = "";
            }

            var query = from comment in _commentRepository
                        join user in _userRepository on comment.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join issue in _isssueRepository on comment.IssueID equals issue.Id into termIssue
                        from i in termIssue.DefaultIfEmpty()
                        where comment.IssueID == id
                        orderby comment.CreationTime descending
                        select new { comment, NameUser = u == null ? null : u.Name, NameIssue = i == null ? null : i.Name };

            var queryResult = query;
            queryResult = queryResult.Skip(input.SkipCount).Take(input.MaxResultCount);
            var result = queryResult.ToList();
            var commentDtos = result.Select(x =>
            {
                var commentDto = ObjectMapper.Map<Comment, CommentDto>(x.comment);
                commentDto.UserName = x.NameUser;
                commentDto.IssueName = x.NameIssue;
                commentDto.datetime = x.comment.CreationTime;
                commentDto.dateModify = x.comment.LastModifierId != null;
                var countAtt = _attachmentRepository.GetListAsync(a => a.TableID == commentDto.Id);
                if (countAtt.Result.Any())
                {
                    commentDto.AttachmentList = new List<AttachmentDto> { };
                    foreach (var att in _attachmentRepository.Where(att => att.TableID == commentDto.Id))
                    {
                        var attDto = ObjectMapper.Map<Attachment, AttachmentDto>(att);
                        commentDto.AttachmentList.Add(attDto);
                    }
                }

                return commentDto;
            }).ToList();
            var totalCount = _commentRepository.Count(x => x.IssueID == id);

            return new PagedResultDto<CommentDto>(
                totalCount,
                commentDtos
            );

        }
        public async Task<ListResultDto<CommentDto>> GetListCommentsByIssueId(Guid issueId)
        {
            var query = from comment in _commentRepository
                        join user in _userRepository on comment.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join issue in _isssueRepository on comment.IssueID equals issue.Id into termIssue
                        from i in termIssue.DefaultIfEmpty()
                        where comment.IssueID == issueId
                        select new { comment, NameUser = u == null ? null : u.Name, NameIssue = i == null ? null : i.Name };
            var queryResult = await AsyncExecuter.ToListAsync(query);
            var commentDtos = queryResult.Select(x =>
            {
                var commentDto = ObjectMapper.Map<Comment, CommentDto>(x.comment);
                commentDto.UserName = x.NameUser;
                commentDto.IssueName = x.NameIssue;
                commentDto.datetime = x.comment.CreationTime;
                commentDto.dateModify = x.comment.LastModifierId != null;
                return commentDto;
            }).ToList();
            var totalCount = _commentRepository.Count(x => x.IssueID == issueId);

            return new PagedResultDto<CommentDto>(
                totalCount,
                commentDtos
                );
        }
        public async Task<ListResultDto<CommentDto>> GetListCommentsByUserId(string userId)
        {
            var query = from comment in _commentRepository
                        join user in _userRepository on comment.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join issue in _isssueRepository on comment.IssueID equals issue.Id into termIssue
                        from i in termIssue.DefaultIfEmpty()
                        where comment.UserID == userId
                        select new { comment, NameUser = u == null ? null : u.Name, NameIssue = i == null ? null : i.Name };
            var queryResult = await AsyncExecuter.ToListAsync(query);
            var commentDtos = queryResult.Select(x =>
            {
                var commentDto = ObjectMapper.Map<Comment, CommentDto>(x.comment);
                commentDto.UserName = x.NameUser;
                commentDto.IssueName = x.NameIssue;
                commentDto.datetime = x.comment.CreationTime;
                commentDto.dateModify = x.comment.LastModifierId != null;
                return commentDto;
            }).ToList();
            var totalCount = _commentRepository.Count(x => x.UserID == userId);

            return new PagedResultDto<CommentDto>(
                totalCount,
                commentDtos
                );
        }
        public async Task InsertAsync(CreateUpdateCommentDto Entity)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var issue = _isssueRepository.FirstOrDefault(x => x.Id == Entity.IssueID);
            var project = await _projectRepository.GetAsync(issue.ProjectID);
            if (project.ProjectIdTFS != Guid.Empty && !_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
            {
                throw new UserFriendlyException("This project has been synchronized with Tfs, please update the information to continue!");
            }
            var comment = _commentManager.Create(
                Entity.IssueID,
                Entity.UserID,
                Entity.Content,
                0
                );
            var newcmt = await _commentRepository.InsertAsync(comment);
            if (Entity.Url[0] != "")
            {
                await _attachmentService.CreateByListAsync(Entity.Url, newcmt.Id);
            }
            if (Entity.Mention.Length > 0)
            {
                await _notificationsAppService.InsertByListMentionAsync(Entity.Mention, Entity.IssueID,
                     _userRepository.FindAsync(Entity.UserID).Result.Name + " has mentioned you in a comment : "
                    + _isssueRepository.FindAsync(Entity.IssueID).Result.Name,
                    _userRepository.FindAsync(Entity.UserID).Result.Name + " has comment on issue you're following: "
                    + _isssueRepository.FindAsync(Entity.IssueID).Result.Name);

            }
            else
            {
                await _notificationsAppService.InsertByListAsync(Entity.IssueID,
                    _userRepository.FindAsync(Entity.UserID).Result.Name + " has comment on issue you're following: "
                    + _isssueRepository.FindAsync(Entity.IssueID).Result.Name);
            }


            if (_azureRepository.Any())
            {
                _backgroundJobClient.Enqueue<IAzureAppService>(x => x.InsertComment(comment.Id, issue.Id, Entity.Content, project.Name, CurrentUserId));
            }

        }

        public async Task UpdateAsync(CreateUpdateCommentDto Entity, Guid Id)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var comment = await _commentRepository.GetAsync(Id);
            var issue = await _isssueRepository.GetAsync(comment.IssueID);
            var project = await _projectRepository.GetAsync(issue.ProjectID);
            if (project.ProjectIdTFS != Guid.Empty && !_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
            {
                throw new UserFriendlyException("This project has been synchronized with Tfs, please update the information to continue!");
            }
            var attachments = await _attachmentRepository.GetListAsync(x => x.TableID == Id);
            if (comment == null)

            {
                throw new EntityNotFoundException(typeof(Comment), Id);
            }
            else
            {
                if (attachments.Any() && Entity.Url[0] != "")
                {
                    for (var i = 0; i < attachments.Count; i++)
                    {
                        await _attachmentRepository.DeleteAsync(attachments[i]);
                    }
                    await _attachmentService.CreateByListAsync(Entity.Url, Id);
                }
                if (attachments.Any() && Entity.Url[0] == "")
                {
                    for (var i = 0; i < attachments.Count; i++)
                    {
                        await _attachmentRepository.DeleteAsync(attachments[i]);
                    }
                }
                if (!attachments.Any() && Entity.Url[0] != "")
                {
                    await _attachmentService.CreateByListAsync(Entity.Url, Id);
                }

                comment.IssueID = Entity.IssueID;
                comment.UserID = Entity.UserID;
                comment.Content = Entity.Content;

                await _commentRepository.UpdateAsync(comment);
                if (_azureRepository.Any())
                {
                    _backgroundJobClient.Enqueue<IAzureAppService>(x => x.UpdateComment(Id, comment.IssueID, Entity.Content, project.Name, CurrentUserId));
                }
            }
        }
        public class MentionCompare : IEqualityComparer<MentionCommentDto>
        {
            public bool Equals(MentionCommentDto x, MentionCommentDto y)
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
                return x.id == y.id;
            }

            public int GetHashCode([DisallowNull] MentionCommentDto obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                //Get the ID hash code value
                int IDHashCode = obj.id.GetHashCode();

                //Get the string HashCode Value
                //Check for null refernece exception

                return IDHashCode;
            }
        }
        /// Danh sach user cmt o issues
        public async Task<List<MentionCommentDto>> GetListUserCmtIssued(Guid issueId)
        {
            MentionCompare mentionCompare = new MentionCompare();
            List<MentionCommentDto> mentionCommentDtos = new List<MentionCommentDto>();
            var issue = _isssueRepository.Where(x => x.Id == issueId).FirstOrDefault();
            var query = from user in _userRepository
                        join assignee in _assigneeRepository on user.Id equals assignee.UserID into termAssignee
                        from a in termAssignee.DefaultIfEmpty()
                        join issues in _isssueRepository on user.Id equals issues.CreatorId into termIssues
                        from i in termIssues.DefaultIfEmpty()
                        join project in _projectRepository on user.Id equals project.CreatorId into termProject
                        from p in termProject.DefaultIfEmpty()
                        where a.IssueID == issueId || i.Id == issueId || issue.ProjectID == p.Id
            select new MentionCommentDto {
                            id = user.Id,
                            value = user.Name
                        };
            var usersDtos = query.Distinct().ToList();
            var ListUser = _userRepository.ToList();
            foreach (AppUser user in ListUser)
            {
                if(await _userManager.CheckRoleByUserId(user.Id, "admin"))
                {
                    var mention = new MentionCommentDto();
                    mention.id = user.Id;
                    mention.value = user.Name;
                    mentionCommentDtos.Add(mention);
                }
            }
            var listUnion = usersDtos.Concat(mentionCommentDtos).ToList();
            var result = listUnion.Distinct(mentionCompare).ToList();
            return result;
        }
    }
}
