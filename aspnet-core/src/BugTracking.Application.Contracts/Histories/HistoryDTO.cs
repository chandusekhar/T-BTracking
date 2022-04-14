using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking
{
    public class HistoryDTO : EntityDto<Guid>
    {

        public string ApplicationName { get; set; }
        public Guid IssueId { get; set; }
        public Guid IdProject { get; set; }
        public Guid AuditLogId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Entity { get; set; }
        public string InProject { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime ExcutitonTime { get; set; }
        public int count { get; set; }
        public string Color { get; set; }
        public string comment { get; set; }
        public string nzColor { get; set; }
    }
}
