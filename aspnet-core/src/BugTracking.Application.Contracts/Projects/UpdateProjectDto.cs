using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Projects
{
    public class UpdateProjectDto
    {

        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        public string NzColor { get; set; }
        public string WitType { get; set; }
        public string Description { get; set; }
    }
}
