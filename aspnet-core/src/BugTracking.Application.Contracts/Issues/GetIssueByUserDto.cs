using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Issues
{
    public class GetIssueByUserDto
    {
        public string Name { get; set; }
        public string ProjectName { get; set; }
        public string StatusName { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
