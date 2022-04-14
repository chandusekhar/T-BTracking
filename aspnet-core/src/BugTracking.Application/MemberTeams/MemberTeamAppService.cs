using BugTracking.Assignees;
using BugTracking.Issues;
using BugTracking.Teams;
using BugTracking.Users;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace BugTracking.MemberTeams
{
    [Authorize("BugTracking.Users")]
    public class MemberTeamAppService : BugTrackingAppService
    {
        private readonly IMemberTeamRepository _memberTeamRepository;
        private readonly MemberTeamManager _memberTeamManager;
        private readonly IIssueRepository _issueRepository;
        private readonly IAssigneeRepository _assigneeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;
        public MemberTeamAppService(
            IMemberTeamRepository memberTeamRepository,
            MemberTeamManager memberTeamManager,
            IIssueRepository issueRepository,
            IAssigneeRepository assigneeRepository,
            IUserRepository userRepository,
            ITeamRepository teamRepository
            )
        {
            _memberTeamRepository = memberTeamRepository;
            _memberTeamManager = memberTeamManager;
            _issueRepository = issueRepository;
            _assigneeRepository = assigneeRepository;
            _userRepository = userRepository;
            _teamRepository = teamRepository;
        }
        public async Task InsertAsync(CreateUpdateMemberTeamDto input)
        {
            if (_memberTeamRepository.Any(x => x.UserID == input.IdUser && x.IdTeam == input.IdTeam))
            {
                throw new UserFriendlyException("User alredy Exist!");
            }
            var memberteam = await _memberTeamManager.CreateAsync(
                input.IdTeam,
                input.IdUser
                );
            await _memberTeamRepository.InsertAsync(memberteam);
        }
        public async Task UpdateAsync(CreateUpdateMemberTeamDto input, Guid Id)
        {
            try
            {
                var memberteam = await _memberTeamRepository.GetAsync(Id);
                if (memberteam == null)

                {
                    throw new EntityNotFoundException(typeof(MemberTeam), Id);
                }
                else
                {
                    memberteam.IdTeam = input.IdTeam;
                    memberteam.UserID = input.IdUser;
                    await _memberTeamRepository.UpdateAsync(memberteam);
                }
            }
            catch
            {
                throw new UserFriendlyException("Xảy ra lỗi khi cập nhật !!");
            }
        }
        public async Task<List<AppUser>> getListUserAddMemberTeam(string idTeam)
        {
            if (idTeam.IsNullOrWhiteSpace() || idTeam == "null")
            {
                idTeam = "";
            }
        
            var members = _memberTeamRepository.Where(x => x.IdTeam.ToString() == idTeam).ToList();
            var listUser = new List<AppUser> { };
            var listUserRM = new List<AppUser> { };
            foreach (MemberTeam member in members)
            {
                var u = _userRepository.Where(x => x.Id == member.UserID).ToList();
                foreach(AppUser use in u)
                {
                    listUserRM.Add(use);
                }
            }
            listUser = await _userRepository.GetListAsync();
            foreach (AppUser us in listUserRM)
            {
                listUser.Remove(us);
            }
            return listUser;
         }
        // get member by team
    public async Task<List<MemberTeamDto>> GetListByIdTeam( string IdTeam)
        {
            if (IdTeam.IsNullOrWhiteSpace() || IdTeam == "null")
            {
                IdTeam = "";
            }
          
                var queryable = await _memberTeamRepository.GetQueryableAsync();
                var query = from memberTeam in queryable
                            where memberTeam.IdTeam.ToString()== IdTeam
                            select new { memberTeam };
                var queryResult = await AsyncExecuter.ToListAsync(query);
                var memberTeamDtos = queryResult.Select(result =>
                {
                    var memberTeamDto = ObjectMapper.Map<MemberTeam, MemberTeamDto>(result.memberTeam);
                    var user = _userRepository.Where(u => u.Id == result.memberTeam.UserID).FirstOrDefault();
                        memberTeamDto.IdUser = user.Id;
                        memberTeamDto.NameMember = user.Name;
                        memberTeamDto.PhoneNumber = user.PhoneNumber;
                        memberTeamDto.Email = user.Email;
                        int issueCreate = _issueRepository.Where(x => x.CreatorId == result.memberTeam.UserID).Count();
                        var listAssign = _assigneeRepository.Where(x => x.UserID == result.memberTeam.UserID).ToList();
                        int issueAssign = 0;
                        foreach (Assignee assign in listAssign)
                        {
                            issueAssign = issueAssign + _issueRepository.Where(x => x.Id == assign.IssueID && x.CreatorId != assign.UserID).Count();
                        }
                        memberTeamDto.CountIssue = issueCreate + issueAssign;
                    return memberTeamDto;
                }).ToList();
                return memberTeamDtos;
        }
        // get member by department
        public async Task<List<MemberTeamDto>> GetListByIdDepartment(string IdDepartment)
        {
            if (IdDepartment.IsNullOrWhiteSpace() || IdDepartment == "null")
            {
                IdDepartment = "";
            }

            var queryable = await _memberTeamRepository.GetQueryableAsync();
            var query = from memberTeam in queryable
                        join team in _teamRepository on memberTeam.IdTeam equals team.Id into termTeam
                        from t in termTeam.DefaultIfEmpty()
                        where t.IdDepartment.ToString() == IdDepartment
                        select new { memberTeam };
            var queryResult = await AsyncExecuter.ToListAsync(query);
            var memberTeamDtos = queryResult.Select(result =>
            {
                var memberTeamDto = ObjectMapper.Map<MemberTeam, MemberTeamDto>(result.memberTeam);
                var user = _userRepository.Where(u => u.Id == result.memberTeam.UserID).FirstOrDefault();
                memberTeamDto.IdUser = user.Id;
                memberTeamDto.NameMember = user.Name;
                memberTeamDto.PhoneNumber = user.PhoneNumber;
                memberTeamDto.Email = user.Email;
                int issueCreate = _issueRepository.Where(x => x.CreatorId == result.memberTeam.UserID).Count();
                var listAssign = _assigneeRepository.Where(x => x.UserID == result.memberTeam.UserID).ToList();
                int issueAssign = 0;
                foreach (Assignee assign in listAssign)
                {
                    issueAssign = issueAssign + _issueRepository.Where(x => x.Id == assign.IssueID && x.CreatorId != assign.UserID).Count();
                }
                memberTeamDto.CountIssue = issueCreate + issueAssign;
                return memberTeamDto;
            }).ToList();
            return memberTeamDtos;
        }
        public async Task DeleteByIdAsync(Guid Id)
        {
            try
            {
                var memberteam = await _memberTeamRepository.GetAsync(Id);
                if (memberteam == null)

                {
                    throw new EntityNotFoundException(typeof(MemberTeam), Id);
                }
                else
                {
                    //var issues = await _issueRepository.
                    //    GetListAsync(x => x.CreatorId == memberteam.UserID );
                    //var assignees = await _assigneeRepository.GetListAsync(x => x.UserID == memberteam.UserID);
                    //if (assignees.Any())
                    //{
                    //    await _assigneeRepository.DeleteManyAsync(assignees);
                    //}

                    //if (issues.Any())
                    //{
                    //    for (int i = 0; i < issues.Count; i++)
                    //    {
                    //        var assigneesIssue = await _assigneeRepository.GetListAsync(x => x.IssueID == issues[i].Id);
                    //        if (assigneesIssue.Any())
                    //        {
                    //            await _assigneeRepository.DeleteManyAsync(assigneesIssue);
                    //        }
                    //        await _issueRepository.DeleteAsync(issues[i].Id);
                    //    }
                    //}
                    await _memberTeamRepository.DeleteAsync(memberteam);
                }
            }
            catch
            {
                throw new UserFriendlyException("Xảy ra lỗi khi xóa !!");
            }
        }
    }
}
