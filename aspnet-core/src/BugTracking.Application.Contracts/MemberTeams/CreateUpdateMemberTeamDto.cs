using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.MemberTeams
{
    public class CreateUpdateMemberTeamDto
    {
        [Required]
        public string IdUser { get; set; }
        [Required]
        public Guid IdTeam { get; set; }
    }
}
