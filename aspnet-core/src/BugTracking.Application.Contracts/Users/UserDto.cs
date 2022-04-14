using System;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Users
{
    public class UserDto 
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid IDMember { get; set; }
        public string CreatedBy { get; set; }
        public int IssueCreated { get; set; }
        public int IssueAssign { get; set; }
        public int IssueOutOfDate { get; set; }
        public int CountIssueOutOfDate { get; set; }
        public long IssueFinish { get; set; }
        public int CountIssueFinish { get; set; }
        public int CountIssue { get; set; }
        public string IdUserAdmin { get; set; }
        public string RoleName { get; set; }
        public int ProjectCreated { get; set; }
    }
}