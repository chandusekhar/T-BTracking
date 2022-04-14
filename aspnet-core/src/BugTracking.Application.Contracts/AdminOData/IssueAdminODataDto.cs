using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.AdminOData
{
    public class IssueAdminODataDto
    {
        public string Id { get; set; }
        public Guid IdIssue { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid StatusID { get; set; }
        public string StatusName { get; set; }
        public Guid? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public DateTime? DueDate { get; set; }
        public int CountIssueDueDate { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserNameAssign { get; set; }
        public Guid ProjectID { get; set; }
        public string ProjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? FinishDate { get; set; }
        public string NzColor { get; set; }
        public string CreatorId { get; set; }
        public int follows { get; set; }
        public int comments { get; set; }
        public int attachments { get; set; }
    }
}
