using BugTracking.Issues;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Projects
{
    public class ItemProjectDTO
    {
        public double AssigneeCount { get; set; }
        public double issueCount { get; set; }
        public double percentAdd { get; set;}
        public double percentIssue { get; set; }
        public double notClosed { get; set; }
        public List<IssuesDto> listIssue { get; set; }
    }
}
