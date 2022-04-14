using BugTracking.Issues;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Notifications
{
    public class NotificationDto : EntityDto<Guid>
    {
        public Guid IssueID { get; set; }
        public string UserID { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public bool IsRead { get; set; }
        public string IssueName { get; set; }
        public IssuesDto issueDto { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorId { get; set; }
        public string Url { get; set; }
    }
}
