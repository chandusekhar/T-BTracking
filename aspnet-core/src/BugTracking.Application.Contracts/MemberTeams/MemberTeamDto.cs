using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.MemberTeams
{
    public class MemberTeamDto 
    {
        public Guid Id { get; set; }
        public string IdUser { get; set; }
        public Guid IdTeam { get; set; }
        public string NameTeam { get; set; }
        public string NameMember { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int CountIssue { get; set; }
    }
}
