using BugTracking.Issues;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Statuss
{
    public class CategoryBoardDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public List<IssuesDto> IssueList { get; set; }
        public int CountIssue { get; set; }
    }
}
