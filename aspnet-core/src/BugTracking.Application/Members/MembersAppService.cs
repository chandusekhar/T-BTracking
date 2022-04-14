using BugTracking.Assignees;
using BugTracking.IShareDto;
using BugTracking.Issues;
using BugTracking.Notifications;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Users;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Members
{
    [Authorize("BugTracking.Users")]
    public class MembersAppService : BugTrackingAppService, IMemberService, IRepositoryCreateUpdateShareDto<CreateUpdateMemberDto, Guid>, IRepositoryDeleteGetByIdShare<MemberDto, Guid>
    {
        
        private readonly IMemberRepository _memberRepository;
        private readonly NotificationsAppService _notificationsAppService;
        private readonly MemberManager _memberManager;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<AppUser, string> _userRepository;
        private readonly IRepository<Assignee, Guid> _assigneeRepository;
        private readonly IRepository<Issue, Guid> _issueRepository;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly UserManager _userManager;
        private readonly IStatusRepository _statusRepository;
        public MembersAppService(IMemberRepository memberRepository,
            MemberManager memberManager,
            IRepository<Project, Guid> projectRepository,
            IRepository<Assignee, Guid> assigneeRepository,
            IRepository<Issue, Guid> issueRepository,
            ITMTCurrentUser itmtCurrentUser,
            IRepository<AppUser, string> userRepository,
            UserManager userManager,
            NotificationsAppService notificationsAppService,
            IStatusRepository statusRepository)
        {
            _memberRepository = memberRepository;
            _memberManager = memberManager;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _assigneeRepository = assigneeRepository;
            _issueRepository = issueRepository;
            _tmtCurrentUser = itmtCurrentUser;
            _userManager = userManager;
            _notificationsAppService = notificationsAppService;
            _statusRepository = statusRepository;
        }
        public async Task<MemberDto> GetByIdAsync(Guid id)
        {
            var query = from member in _memberRepository
                        join user in _userRepository on member.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join project in _projectRepository on member.ProjectID equals project.Id
                        where member.Id == id
                        select new { member, NameUser = u == null ? null : u.Name, project };

            var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);
            if (queryResult == null)
            {
                throw new EntityNotFoundException(typeof(Member), id);
            }

            var memberDto = ObjectMapper.Map<Member, MemberDto>(queryResult.member);
            memberDto.UserName = queryResult.NameUser;
            memberDto.ProjectName = queryResult.project.Name;
            return memberDto;
        }
        public async Task<PagedResultDto<MemberDto>> GetListAsync(GetListDto input)
        {
           
            var query = from member in _memberRepository
                        join user in _userRepository on member.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join project in _projectRepository on member.ProjectID equals project.Id
                        orderby input.Sorting
                        select new { member, NameUser = u == null ? null : u.Name, project };
           
            var queryResult = await AsyncExecuter.ToListAsync(query);

            var memberDtos = queryResult.Select(x =>
            {
                var memberDto = ObjectMapper.Map<Member, MemberDto>(x.member);
                memberDto.UserName = x.NameUser;
                memberDto.ProjectName = x.project.Name;
                return memberDto;
            }).ToList();
            var totalCount = await _memberRepository.GetCountAsync();
            return new PagedResultDto<MemberDto>(
                totalCount,
                memberDtos
            );
        }
        public async Task<ListResultDto<MemberDto>> GetListMembersByProjectId(Guid projectId)
        {
            var query = from member in _memberRepository
                        join user in _userRepository on member.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join project in _projectRepository on member.ProjectID equals project.Id
                        where member.ProjectID == projectId
                        select new { member, NameUser = u == null ? null : u.Name, project };
            var queryResult = await AsyncExecuter.ToListAsync(query);
            var memberDtos = queryResult.Select(x =>
            {
                var memberDto = ObjectMapper.Map<Member, MemberDto>(x.member);
                memberDto.UserName = x.NameUser;
                memberDto.ProjectName = x.project.Name;
              
                return memberDto;
            }).ToList();
            var totalCount = _memberRepository.Count(x => x.ProjectID == projectId);

            return new PagedResultDto<MemberDto>(
                totalCount,
                memberDtos
            );
        }
        public async Task<ListResultDto<MemberDto>> GetListMembersByUserId(string userId)
        {
            var query = from member in _memberRepository
                        join user in _userRepository on member.UserID equals user.Id into termUser
                        from u in termUser.DefaultIfEmpty()
                        join project in _projectRepository on member.ProjectID equals project.Id
                        where member.UserID == userId
                        select new { member, NameUser = u == null ? null : u.Name, project };
            var queryResult = await AsyncExecuter.ToListAsync(query);
            var memberDtos = queryResult.Select(x =>
            {
                var memberDto = ObjectMapper.Map<Member, MemberDto>(x.member);
                memberDto.UserName = x.NameUser;
                memberDto.ProjectName = x.project.Name;
                return memberDto;
            }).ToList();
            var totalCount = _memberRepository.Count(x => x.UserID == userId);

            return new PagedResultDto<MemberDto>(
                totalCount,
                memberDtos
            );
        }
        public async Task InsertAsync(CreateUpdateMemberDto Entity)
        {
            if (_memberRepository.Any(x=>x.UserID == Entity.UserID && x.ProjectID == Entity.ProjectID))
            {
                throw new UserFriendlyException("User alredy Exist!");
            }
            var member = await _memberManager.CreateAsync(
                Entity.ProjectID,
                Entity.UserID
                );
            await _memberRepository.InsertAsync(member);
        }
        public async Task InsertByListAsync(CreateMemberByListDto Entity)
        {
            foreach(string userId in Entity.UserIDs)
            {
            if (_memberRepository.Any(x => x.UserID == userId && x.ProjectID == Entity.ProjectID))
            {
                    continue;
            }
            var member = await _memberManager.CreateAsync(
                Entity.ProjectID,
                userId
                );
            await _memberRepository.InsertAsync(member);
            }
        }
        [AllowAnonymous]
        public async Task InsertAnonymousAsync(CreateUpdateMemberDto Entity)
        {
            if (_memberRepository.Any(x => x.UserID == Entity.UserID && x.ProjectID == Entity.ProjectID))
            {
                throw new UserFriendlyException("User alredy Exist!");
            }
            var member = await _memberManager.CreateAsync(
                Entity.ProjectID,
                Entity.UserID
                );
            await _memberRepository.InsertAsync(member);
        }
        public async Task UpdateAsync(CreateUpdateMemberDto Entity, Guid Id)
        {
            try
            {
                var member = await _memberRepository.GetAsync(Id);
                if (member == null)

                {
                    throw new EntityNotFoundException(typeof(Member), Id);
                }
                else
                {
                    member.ProjectID = Entity.ProjectID;
                    member.UserID = Entity.UserID;


                    await _memberRepository.UpdateAsync(member);

                }
            }
            catch
            {
                throw new UserFriendlyException("Xảy ra lỗi khi cập nhật !!");
            }
        }
        public async Task DeleteByIdAsync(Guid Id)
        {
            try
            {
                var member = await _memberRepository.GetAsync(Id);
                if (member == null)

                {
                    throw new EntityNotFoundException(typeof(Member), Id);
                }
                else
                {
                    var issues =await _issueRepository.
                        GetListAsync(x => x.CreatorId == member.UserID && x.ProjectID==member.ProjectID);
                    var assignees = await _assigneeRepository.GetListAsync(x => x.UserID == member.UserID);
                    if (assignees.Any())
                    {
                            await _assigneeRepository.DeleteManyAsync(assignees);
                    }
                   
                    if (issues.Any())
                    {
                        for (int i = 0; i < issues.Count; i++)
                        {
                            var assigneesIssue =await _assigneeRepository.GetListAsync(x => x.IssueID == issues[i].Id);
                            if (assigneesIssue.Any())
                            {
                                await _assigneeRepository.DeleteManyAsync(assigneesIssue);
                            }
                            await _issueRepository.DeleteAsync(issues[i].Id);
                        }
                    }
                    await _memberRepository.DeleteAsync(member);

                }
            }
            catch
            {
                throw new UserFriendlyException("Xảy ra lỗi khi xóa !!");
            }
        }
        public async Task<bool> GetCheckMember(Guid projectId)
        {
            var CurrentUserId=_tmtCurrentUser.GetId();
             
            if (_memberRepository.Any(x=>x.UserID== CurrentUserId && x.ProjectID==projectId) 
                || await _userManager.CheckRoleByUserId(CurrentUserId, "admin"))
            {
                return true;
            }
            else return false;
        }

        public class MemberAndEnityStatistics
        {
            public List<MemberStatisticsDto> memberStatisticsDtos { get; set; }
            public List<EntityStatisticsDto> entityStatisticsDto { get; set; }
        }

        public async Task<MemberAndEnityStatistics> GetMemberStatisticsAsync(Guid projectId)
        {
            var newestCreated = _issueRepository.Where(x => x.ProjectID == projectId)
                .OrderByDescending(x => x.CreationTime)
                .FirstOrDefault();

            var newestClosed = _issueRepository.Where(x => x.ProjectID == projectId && x.FinishDate != null)
                .OrderByDescending(x => x.FinishDate)
                .FirstOrDefault();

            var completeLatest = _issueRepository.Where(x => x.ProjectID == projectId && x.FinishDate != null)
                .ToList().
                OrderByDescending(x => (x.FinishDate - x.StartDate).Value.TotalMilliseconds)
                .FirstOrDefault();

            var completeFasted = _issueRepository.Where(x => x.ProjectID == projectId && x.FinishDate != null)
                .ToList().
                OrderBy(x => (x.FinishDate - x.StartDate).Value.TotalMilliseconds)
                .FirstOrDefault();

            var theMostChild = _issueRepository.Where(x => x.ProjectID == projectId)
                .OrderByDescending(x=> _issueRepository.Count(c => c.IdParent == x.Id))
                .FirstOrDefault(x => _issueRepository.Count(c => c.IdParent == x.Id) > 0);

            var nearestDueDate = _issueRepository
                .Where(x => x.ProjectID == projectId && x.DueDate != null && _statusRepository.FirstOrDefault(s=>s.Id == x.StatusID).Name != "Closed" && x.DueDate >= DateTime.UtcNow)
                .OrderByDescending(x => x.DueDate)
                .FirstOrDefault();

            var farestDueDate = _issueRepository
                .Where(x => x.ProjectID == projectId && x.DueDate != null && _statusRepository.FirstOrDefault(s => s.Id == x.StatusID).Name != "Closed" && x.DueDate < DateTime.UtcNow)
                .OrderBy(x => x.DueDate)
                .FirstOrDefault();


            List<EntityStatisticsDto> entityStatisticsDtos = new List<EntityStatisticsDto>() {};

            if (newestCreated != null)
            {
                entityStatisticsDtos.Add(new EntityStatisticsDto
                {
                    Type = "Newest Created",
                    Entity = newestCreated != null ? newestCreated.Name : "",
                    By = newestCreated != null ? (newestCreated.LastModifierId != null ? _userRepository.FirstOrDefault(x => x.Id == newestCreated.LastModifierId).Name : _userRepository.FirstOrDefault(x => x.Id == newestCreated.CreatorId).Name) : "Unknown",
                    Time = newestCreated != null ? ((DateTime)newestCreated.CreationTime).ToString("dd/MM/yyyy HH:mm:ss") : 0,
                    Id = newestCreated.Id
                });
            }

            if (newestClosed != null)
            {
                entityStatisticsDtos.Add(new EntityStatisticsDto
                {
                    Type = "Newest Closed",
                    Entity = newestClosed != null ? newestClosed.Name : "",
                    By = newestClosed != null ? (newestClosed.LastModifierId != null ? _userRepository.FirstOrDefault(x => x.Id == newestClosed.LastModifierId).Name : _userRepository.FirstOrDefault(x => x.Id == completeFasted.CreatorId).Name) : "Unknown",
                    Time = newestClosed != null ? ((DateTime)newestClosed.FinishDate).ToString("dd/MM/yyyy HH:mm:ss") : 0,
                    Id = newestClosed.Id
                });
            }

            if(completeLatest != null)
            {
                entityStatisticsDtos.Add(new EntityStatisticsDto
                {
                    Type = "Completed Latest",
                    Entity = completeLatest != null ? completeLatest.Name : "",
                    By = completeLatest.CreatorId != null ? (completeLatest.LastModifierId != null ? _userRepository.FirstOrDefault(x => x.Id == completeLatest.LastModifierId).Name : _userRepository.FirstOrDefault(x => x.Id == completeFasted.CreatorId).Name) : "Unknown",
                    Time = completeLatest != null ? Math.Round((completeLatest.FinishDate - completeLatest.StartDate).Value.TotalDays, 2).ToString() + " Days" : 0,
                    Id = completeLatest.Id
                });
            }

            if(completeFasted != null)
            {
                entityStatisticsDtos.Add(new EntityStatisticsDto
                {
                    Type = "Completed Fastest",
                    Entity = completeFasted != null ? completeFasted.Name : "",
                    By = completeFasted != null ? (completeFasted.LastModifierId != null ? _userRepository.FirstOrDefault(x => x.Id == completeFasted.LastModifierId).Name : 
                    _userRepository.FirstOrDefault(x => x.Id == completeFasted.CreatorId).Name) : "Unknown",
                    Time = completeFasted != null ? ((completeFasted.FinishDate - completeFasted.StartDate).Value.Days.ToString() + " Days "
                    + (completeFasted.FinishDate - completeFasted.StartDate).Value.Minutes.ToString() + " Minutes "
                    + (completeFasted.FinishDate - completeFasted.StartDate).Value.Seconds.ToString() + " Seconds") : 0,
                    Id = completeFasted.Id
                });
            }

            if (theMostChild != null)
            {
                entityStatisticsDtos.Add(new EntityStatisticsDto
                {
                    Type = "The Most Task Child",
                    Entity = theMostChild != null ? theMostChild.Name : "",
                    By = theMostChild != null ? (theMostChild.LastModifierId != null ? _userRepository.FirstOrDefault(x => x.Id == theMostChild.LastModifierId).Name : 
                     _userRepository.FirstOrDefault(x => x.Id == theMostChild.CreatorId).Name) : "Unknown",
                    Time = theMostChild != null ? _issueRepository.Count(x=>x.IdParent == theMostChild.Id).ToString() + " Tasks Child" : 0,
                    Id = theMostChild.Id
                });
            }

            if (nearestDueDate != null)
            {
                entityStatisticsDtos.Add(new EntityStatisticsDto
                {
                    Type = "Nearest Upcoming DueDate",
                    Entity = nearestDueDate != null ? nearestDueDate.Name : "",
                    By = nearestDueDate != null ? (nearestDueDate.LastModifierId != null ? _userRepository.FirstOrDefault(x => x.Id == nearestDueDate.LastModifierId).Name : 
                    _userRepository.FirstOrDefault(x => x.Id == nearestDueDate.CreatorId).Name) : "Unknown",
                    Time = nearestDueDate != null ? ((DateTime)nearestDueDate.DueDate).ToString("dd/MM/yyyy") : 0,
                    Id = nearestDueDate.Id
                });
            }

            if (farestDueDate != null)
            {
                entityStatisticsDtos.Add(new EntityStatisticsDto
                {
                    Type = "Farest DueDate",
                    Entity = farestDueDate != null ? farestDueDate.Name : "",
                    By = farestDueDate != null ? (farestDueDate.LastModifierId != null ? _userRepository.FirstOrDefault(x => x.Id == farestDueDate.LastModifierId).Name : 
                    _userRepository.FirstOrDefault(x => x.Id == farestDueDate.CreatorId).Name) : "Unknown",
                    Time = farestDueDate != null ? ((DateTime)farestDueDate.DueDate).ToString("dd/MM/yyyy") : 0,
                    Id = farestDueDate.Id
                });
            }


            List<MemberStatisticsDto> memberStatistics = new List<MemberStatisticsDto>();

            var members = await _memberRepository.GetListAsync(x=>x.ProjectID == projectId);
            decimal totalBugs = _issueRepository.Count(x => x.ProjectID == projectId);

            foreach(Member member in members)
            {
                var memberMap = ObjectMapper.Map<Member, MemberStatisticsDto>(member);

                memberMap.Name = _userRepository.FirstOrDefault(x => x.Id == member.UserID).Name;

                memberMap.Create = _issueRepository.Count(x => x.CreatorId == member.UserID && x.ProjectID == projectId && 
                ((x.IdParent == Guid.Empty && !_issueRepository.Any(a => a.IdParent == x.Id)) || x.IdParent != Guid.Empty));
                memberMap.Tag = (from project in _projectRepository
                                 join memberJ in _memberRepository on project.Id equals memberJ.ProjectID
                                 join issue in _issueRepository on project.Id equals issue.ProjectID
                                 join asssignee in _assigneeRepository on issue.Id equals asssignee.IssueID
                                 where project.Id == projectId && memberJ.UserID == member.UserID && asssignee.UserID == member.UserID
                                 && ((issue.IdParent == Guid.Empty && !_issueRepository.Any(x => x.IdParent == issue.Id)) || issue.IdParent != Guid.Empty)
                                 select new { issue }).Count();
                memberMap.Close = (from project in _projectRepository
                                   join memberJ in _memberRepository on project.Id equals memberJ.ProjectID
                                   join issue in _issueRepository on project.Id equals issue.ProjectID
                                   join asssignee in _assigneeRepository on issue.Id equals asssignee.IssueID
                                   join status in _statusRepository on issue.StatusID equals status.Id
                                   where project.Id == projectId && memberJ.UserID == member.UserID && asssignee.UserID == member.UserID && status.Name == "Closed"
                                   && ((issue.IdParent == Guid.Empty && !_issueRepository.Any(x => x.IdParent == issue.Id)) || issue.IdParent != Guid.Empty)
                                   select new { issue }).Count();


                memberMap.pCreate = totalBugs > 0 ? Math.Round(((decimal)memberMap.Create / totalBugs) * 100, 0) : 0;
                memberMap.pTag = totalBugs > 0 ? Math.Round(((decimal)memberMap.Tag / totalBugs) * 100, 0) : 0;
                memberMap.pClose = totalBugs > 0 ? Math.Round(((decimal)memberMap.Close / totalBugs) * 100, 0) : 0;

                memberStatistics.Add(memberMap);

                if (memberMap.pCreate == 0 && memberMap.pClose == 0 && memberMap.pTag == 0)
                {
                    memberStatistics.Remove(memberMap);
                }

            }

            return new MemberAndEnityStatistics
            {
                memberStatisticsDtos = memberStatistics,
                entityStatisticsDto = entityStatisticsDtos ?? null
            };
        }

    }
}
