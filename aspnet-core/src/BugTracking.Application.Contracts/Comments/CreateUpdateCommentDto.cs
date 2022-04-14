using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Comments
{
    public class CreateUpdateCommentDto
    {
        [Required]
        public Guid IssueID { get; set; }
        [Required]
        public string UserID { get; set; }
        public string Content { get; set; }
        public string[] Url { get; set; }
        public string[] Mention { get; set; }
    }
}
