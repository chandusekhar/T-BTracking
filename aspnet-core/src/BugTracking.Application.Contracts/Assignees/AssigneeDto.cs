using BugTracking.Attachments;
using BugTracking.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Assignees
{
    public class AssigneeDto
    {
        public Guid ID { get; set; }
        public Guid IssueID { get; set; }
        public string IssueName { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string CreatorID { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string StatusName { get; set; }
        public bool IsClose { get; set; }
        public string NzColor { get; set; }
        public string Description { get; set; }
        public string PriorityValue { get; set; }
        public string LevelValue { get; set; }
        public string Project { get; set; }
        public List<Users.UserDto> ListUser { get; set; }
        public List<AttachmentDto> AttachmentListImage { get; set; }
        public List<AttachmentDto> AttachmentListVideo { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifier { get; set; }
        public Guid ProjectID { get; set; }


    }
}
