using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Projects
{
    public class ProjectStatisticDto
    {
        public int Project { get; set; }
        public int User { get; set; }
        public int Bugs { get; set; }
        public int TotalBug { get; set; }
        public int Close { get; set; }
        public int Open { get; set; }
        public int NotClose { get; set; }
        public int Tag { get; set; }
        public int Create { get; set; }
        public int DueDate { get; set; }
        public int Comment { get; set; }
        public string[] Projects { get; set; }
        public int[] IssuesClosed { get; set; }
        public int[] IssuesCreated { get; set; }
        public int[] IssuesOverDueDate { get; set; }
    }
}
