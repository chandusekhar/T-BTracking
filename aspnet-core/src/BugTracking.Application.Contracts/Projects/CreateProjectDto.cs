using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Projects
{
    public class CreateProjectDto
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
    }
}
