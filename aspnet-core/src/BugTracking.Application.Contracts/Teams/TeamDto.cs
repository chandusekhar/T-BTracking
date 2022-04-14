using BugTracking.MemberTeams;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Teams
{
    public class TeamDto 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IdLeader { get; set; }
        public string NameLeader { get; set; }
        public int CountIssue{ get; set; }
        public Guid IdDepartment { get; set; }
        public string NameDepartment { get; set; }
        public int CountMember { get; set; }
        public List<MemberTeamDto> ListMemberTeam { get; set; }
    }
}
