using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Assignees
{
    public class AssigneeParentDto
    {
        public bool IsHaveParent { get; set; }
        public Guid ParentId { get; set; }
    }
}
