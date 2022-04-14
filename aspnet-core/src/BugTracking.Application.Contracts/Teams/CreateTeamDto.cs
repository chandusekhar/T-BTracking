using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Teams
{
    public class CreateTeamDto
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        [Required]
        public string IdLeader { get; set; }
        [Required]
        public Guid IdDepartment { get; set; }
    }
}
