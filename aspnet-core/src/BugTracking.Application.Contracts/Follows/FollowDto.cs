using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Follows
{
    public class FollowDto
    {
        public Guid ID { get; set; }
        public Guid IssueID { get; set; }
        public string IssueName { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}
