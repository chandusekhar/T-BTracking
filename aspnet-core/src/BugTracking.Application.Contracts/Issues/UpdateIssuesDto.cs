using BugTracking.Attachments;
using BugTracking.DetailAttachment;
using BugTracking.LevelEnum;
using BugTracking.PriorityEnum;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Issues
{
    public class UpdateIssuesDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }//
        public Guid? CategoryID { get; set; }
        public DateTime? DueDate { get; set; }
        public Level IssueLevel { get; set; }
        public Guid ProjectID { get; set; }
        public List<AttachmentDto> Attachments { get; set; }
        public string[] Assignees { get; set; }
        public string[] NotifyMail { get; set; }
        public Guid IdParent { get; set; }
    }
}
