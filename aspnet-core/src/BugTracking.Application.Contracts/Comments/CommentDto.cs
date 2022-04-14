using BugTracking.Attachments;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Comments
{
    public class CommentDto : EntityDto<Guid>
    {
        public Guid IssueID { get; set; }
        public string UserID { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public string IssueName { get; set; }
        public DateTime datetime { get; set; }
        public bool dateModify { get; set; }
        public List<AttachmentDto> AttachmentList { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}
