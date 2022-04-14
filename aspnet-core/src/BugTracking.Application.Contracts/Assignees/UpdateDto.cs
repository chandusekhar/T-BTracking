using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Assignees
{
    public class UpdateDto
    {
        [Required]
        public Guid IssueID { get; set; }

        [Required]
        public string UserID { get; set; }
    }
}
