using BugTracking.LevelEnum;
using BugTracking.PriorityEnum;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Follows
{
    public class IssuePropertiesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid StatusID { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid ProjectID { get; set; }
        public DateTime? FinishDate { get; set; }
    }
    public class IssuePropertiesChangeableDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid StatusID { get; set; }
        public Priority Priority { get; set; }
        public Guid? CategoryID { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid ProjectID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public Level IssueLevel { get; set; }
    }
    public class CommentPropertiesChangeableDto
    {
        public string Content { get; set; }
        public Guid IssueID { get; set; }
    }
    public class AssigneePropertiesChangeableDto
    {
        public Guid IssueID { get; set; }
    }
    public class PropertiesChangeDto
    {
        public string PropertyName { get; set; }
        public string OriginalValue { get; set; }
        public string NewValue { get; set; }
    }
}