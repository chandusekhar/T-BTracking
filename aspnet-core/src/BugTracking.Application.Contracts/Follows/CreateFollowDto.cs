using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Follows
{
    public class CreateFollowDto
    {
        [Required]
        public Guid IssueID { get; set; }

        [Required]
        public string UserID { get; set; }
    }
}
