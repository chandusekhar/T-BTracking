using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Teams
{
    public class UpdateTeamDto
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        [Required]
        public string IdLeader { get; set; }
    }
}
