using BugTracking.DetailAttachment;
using BugTracking.LevelEnum;
using BugTracking.PriorityEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Issues
{
    public class CreateUpdateIssuesDto
    {
        [Required]
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
        public List<CreateDetailAttachmentDto> Attachments { get; set; }
        public string[] Assignees { get; set; }
        public string[] NotifyMail { get; set; }
        public Guid IdParent { get; set; }
        public int IdWit { get; set; }
    }
}
