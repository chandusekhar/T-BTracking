using BugTracking.PriorityEnum;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Follows
{
    public class Calendar_IssueListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set;}
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Priority Priority { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string BackgroundColor { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public TimeSpan? TimeLeft { get; set; }
        public object LastUpdate { get; set; }
    }
}
