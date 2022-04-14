using BugTracking.Assignees;
using BugTracking.Follows;
using BugTracking.Hub;
using BugTracking.IShareDto;
using BugTracking.Issues;
using BugTracking.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Notifications
{
    [Authorize("BugTracking.Users")]
    public class NotificationsAppService : BugTrackingAppService, INotificationService, IRepositoryDeleteGetByIdShare<NotificationDto, Guid>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IAssigneeRepository _assigneeRepository;
        private readonly NotificationManager _notificationManager;
        private readonly IRepository<Issue, Guid> _isssueRepository;
        private readonly IRepository<AppUser, string> _userRepository;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly IFollowRepository _followRepository;
        private IHubContext<SignalR> _hub;
        public NotificationsAppService(INotificationRepository notificationRepository, 
            NotificationManager notificationManager, IRepository<Issue, Guid> isssueRepository, 
            IRepository<AppUser, string> userRepository, ITMTCurrentUser tmtCurrentUser, IFollowRepository followRepository,
            IAssigneeRepository assigneeRepository, IHubContext<SignalR> hub)
        {
            _isssueRepository = isssueRepository;
            _notificationRepository = notificationRepository;
            _notificationManager = notificationManager;
            _userRepository = userRepository;
            _tmtCurrentUser = tmtCurrentUser;
            _followRepository = followRepository;
            _assigneeRepository = assigneeRepository;
            _hub = hub;
        }
        public async Task<NotificationDto> GetByIdAsync(Guid id)
        {
            var query = from notify in _notificationRepository
                        join user in _userRepository on notify.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join issue in _isssueRepository on notify.IssueID equals issue.Id into termIssue
                        from i in termIssue.DefaultIfEmpty()
                        where notify.Id == id
                        select new { notify, NameUser = u == null ? null : u.Name, NameIssue = i == null ? null:i.Name };

            var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);
            if (queryResult == null)
            {
                throw new EntityNotFoundException(typeof(Notification), id);
            }

            var notifyDto = ObjectMapper.Map<Notification, NotificationDto>(queryResult.notify);
            notifyDto.UserName = queryResult.NameUser;
            notifyDto.IssueName = queryResult.NameIssue;
            return notifyDto;
        }
        public MessageNotifyDto GetNewestNotify()
        {
            try
            {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var newestCreationTime = _notificationRepository.
                Where(x => x.UserID == CurrentUserId && x.IsRead == false).Max(x=>x.CreationTime);
            var notify = _notificationRepository.First(x => x.CreationTime == newestCreationTime);
            return ObjectMapper.Map<Notification, MessageNotifyDto>(notify);
            }
            catch { return null; }
        }
        public async Task<ListResultDto<NotificationDto>> GetListAsync(GetListDto input)
        {
            var query = from notify in _notificationRepository
                        join user in _userRepository on notify.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join issue in _isssueRepository on notify.IssueID equals issue.Id into termIssue
                        from i in termIssue.DefaultIfEmpty()
                        orderby input.Sorting
                        select new { notify, NameUser = u == null ? null : u.Name, NameIssue = i == null ? null : i.Name };
            
            var queryResult = await AsyncExecuter.ToListAsync(query);

            var notifyDtos = queryResult.Select(x =>
            {
                var notifyDto = ObjectMapper.Map<Notification, NotificationDto>(x.notify);
                notifyDto.UserName = x.NameUser;
                notifyDto.IssueName = x.NameIssue;
                return notifyDto;
            }).ToList();
            var totalCount = await _notificationRepository.GetCountAsync();
            return new PagedResultDto<NotificationDto>(
                totalCount,
                notifyDtos
            );

        }
        public class CountUnreadNotify
        {
            public int UnRead { get; set; }
            public PagedResultDto<NotificationDto> ResultRequestDto { get; set; }
        }
        public async Task<CountUnreadNotify> GetListNotification(int skip, int take, string currentUserId)
        {
            var query = from notify in _notificationRepository
                        join issue in _isssueRepository on notify.IssueID equals issue.Id
                       where notify.UserID == currentUserId
                        orderby notify.IsRead ascending,notify.CreationTime descending
                        select new { notify, issue };
            var CountUnRead = _notificationRepository.Count(x => x.UserID == currentUserId && x.IsRead == false);
            query = query.Skip(skip).Take(take);
            var queryResult = await AsyncExecuter.ToListAsync(query);
            var notifyDtos = queryResult.Select(x =>
            {
                var notifyDto = ObjectMapper.Map<Notification, NotificationDto>(x.notify);
                notifyDto.UserName = x.notify.CreatorId != null ? _userRepository.FirstOrDefault(u=>u.Id == x.notify.CreatorId).Name : string.Empty;
                notifyDto.IssueName = x.issue.Name;
                return notifyDto;
            }).OrderByDescending(x=>x.CreationTime).ToList();
            var totalCount = _notificationRepository.Count(x => x.UserID == currentUserId);
            var result= new PagedResultDto<NotificationDto>(
                totalCount,
                notifyDtos
                );
            return new CountUnreadNotify
            {
                UnRead = CountUnRead,
                ResultRequestDto = result
            };
        }
        public async Task InsertAsync(Guid IssueId,string UserId,string Message)
        {
            var notify = await _notificationManager.CreateAsync(
                IssueId,
                UserId,
                Message,
                false
                );
            await _notificationRepository.InsertAsync(notify);
        }
        public async Task InsertByListAsync(string[] Users,Guid IssueId,string Message)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            foreach(string user in Users)
            {
                if (_assigneeRepository.Any(x => x.UserID == user && x.IssueID == IssueId))
                {
                    var notify = await _notificationManager.CreateAsync(
                IssueId,
                user,
                "New notify from "+_userRepository.FirstOrDefault(x=>x.Id==CurrentUserId).Name+": Issue "
                +_isssueRepository.FirstOrDefault(x=>x.Id==IssueId).Name+" has been update!",
                false
                );
                    await _notificationRepository.InsertAsync(notify);
                }
                else
                {
                var notify = await _notificationManager.CreateAsync(
                IssueId,
                user,
                Message,
                false
                );
                await _notificationRepository.InsertAsync(notify);
                }
            }
        }
        public async Task InsertByListMentionAsync(string[] Users, Guid IssueId, string Message1, string Message2)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            var listFollow = await _followRepository.GetListAsync(x => x.IssueID == IssueId);
            var listFl = listFollow.Select(x => x.UserID);
            if (listFl.Count() > 0)
            {
                var listUnion = listFl.Union(Users);
                var listExcept = listUnion.Except(Users);
                if (listExcept.Count() > 0)
                {
                    foreach ( string l in listExcept)
                    {
                        var notify = await _notificationManager.CreateAsync(
                                IssueId,
                                l,
                                Message2,
                                false
                                );
                        await _notificationRepository.InsertAsync(notify);
                        await _hub.Clients.All.SendAsync("ReloadNotify", l);
                    }
                }

                foreach (string user in Users)
                {
                    var notify = await _notificationManager.CreateAsync(
                            IssueId,
                            user,
                            Message1,
                            false
                            );
                    await _notificationRepository.InsertAsync(notify);
                    await _hub.Clients.All.SendAsync("ReloadNotify", user);
                }
                
            }
            else
            {
                foreach (string user in Users)
                {
                    var notify = await _notificationManager.CreateAsync(
                            IssueId,
                            user,
                            Message1,
                            false
                            );
                    await _notificationRepository.InsertAsync(notify);
                }
            }
        }
        internal async Task InsertByListAsync(Guid issueID, string message)
        {
            var listFollow = await _followRepository.GetListAsync(x => x.IssueID == issueID);
            if (listFollow != null)
            {
            foreach (Follow follow in listFollow)
            {
                var notify = await _notificationManager.CreateAsync(
                issueID,
                follow.UserID,
                message,
                false
                );
               await _notificationRepository.InsertAsync(notify);
                
            }
            }
        }

        public async Task DeleteByIdAsync(Guid Id)
        {
            try
            {
                var notify = await _notificationRepository.GetAsync(Id);
                if (notify == null)

                {
                    throw new EntityNotFoundException(typeof(Notification), Id);
                }
                else
                {

                    await _notificationRepository.DeleteAsync(notify);

                }
            }
            catch
            {
                throw new UserFriendlyException("An Error while try to delete !!");
            }
        }

        public async Task GetUpdateAsync()
        {
            try
            {
                var CurrentUserId = _tmtCurrentUser.GetId();
                var listNotify = await _notificationRepository.GetListAsync(x => x.UserID == CurrentUserId && !x.IsRead);

                listNotify.ForEach(x => x.IsRead = true);

                await _notificationRepository.UpdateManyAsync(listNotify);
            }
            catch
            {
                throw new UserFriendlyException("An Error while try to update !!");
            }
        }
        public async Task GetUpdateSingleAsync(Guid Id)
        {
            try
            {
                var notification = await _notificationRepository.GetAsync(Id);
                notification.IsRead = true;
                await _notificationRepository.UpdateAsync(notification);
            }
            catch
            {
                throw new UserFriendlyException("An Error while try to update !!");
            }
        }

    }
}
