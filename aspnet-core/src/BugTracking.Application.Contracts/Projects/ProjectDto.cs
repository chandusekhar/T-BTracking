using BugTracking.Members;
using System;
using System.Collections.Generic;

namespace BugTracking.Projects
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string NzColor { get; set; }
        public int CountMember { get; set; }
        public int countIssue { get; set; }
        public int countIssueClose { get; set; }
        public int issueClose { get; set; }
        public int countIssueDueDateATime { get; set; }
        public int issueDueDateATime { get; set; }
        public int countIssueDueDate { get; set; }
        public int issueDueDate { get; set; }
        public string userName { get; set; }
        public int CountAdd { get; set; }
        public List<MemberDto> MemberList { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string IdUserAdmin { get; set; }
        public Guid ProjectIdTFS { get; set; }
        public string WitType { get; set; }
        public bool IsSynced { get; set; }
    }
}
