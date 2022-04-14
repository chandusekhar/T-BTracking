using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Notifications
{
    public class CreateUpdateNotificationDto
    {
        [Required]
        public Guid IssueID { get; set; }
        public string UserID { get; set; }
        public bool IsRead { get; set; }
    }
}
