using BugTracking.Assignees;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Histories
{
    public class IssueDetailCalendar
    {
        public Guid Id { get; set; }
        public Guid ProjectID { get; set; }
        public string Name { get; set; }
        public string status { get; set; }
        public string ProjectName { get; set; }
        public List<AssigneeDto> ListAssignee { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
