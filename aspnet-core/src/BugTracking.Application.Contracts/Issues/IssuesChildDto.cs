using BugTracking.Assignees;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Issues
{
    public class IssuesChildDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string StatusName { get; set; }
        public List<AssigneeDto> AssigneesList { get; set; }
    }
}
