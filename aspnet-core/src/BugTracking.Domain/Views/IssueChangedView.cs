using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace BugTracking.Views
{
    //[Table("issueChangedViews")]
    public class IssueChangedView : Entity
    {
        public override object[] GetKeys() { return null; }
        public Guid AuditLogId { get; set; }
        //public string EntityId { get; set; }
        //public string EntityTypeFullName { get; set; }=
        //public string NewValue { get; set; }
        //public string OriginalValue { get; set; }
        //public string PropertyName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime ExecutionTime { get; set; }
        public string HttpMethod { get; set; }
        public string IssueId { get; set; }
        public string IssueName { get; set; }
        //public string IssueCategoryID { get; set; }
        //public string IssueCategoryName { get; set; }
        //public string IssueDescription { get; set; }
        public string IssueDueDate { get; set; }
        public string IssueFinishDate { get; set; }
        //public string IssueLevel { get; set; }
        //public string IssuePriority { get; set; }
        public string IssueProjectID { get; set; }
        public string IssueProjectName { get; set; }
        public string IssueStatusID { get; set; }
        public string IssueStatusName { get; set; }
        public string IssueStatusColor { get; set; }
        public string ListChange { get; set; }
        //public string IssueCreationTime { get; set; }
    }
}
