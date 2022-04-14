using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.AdminOData
{
    public class AdminODataDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreationTime { get; set; }
        public int IssueCreated { get; set; }
        public int ProjectCreate { get; set; }
        public int CountIssueAssign { get; set; }
        public int TotalIssue { get; set; }
        public int IssueDueDate { get; set; }
        public int IssueFinish { get; set; }
        public string UserId { get; set; }
        public string userName { get; set; }
        public int CountMember { get; set; }
    }
}
