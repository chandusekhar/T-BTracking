using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Departments
{
    public class DepartmentDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string IdManager { get; set; }
        public string NameManager { get; set; }
        public int CountMember { get; set; }
        public int CountIssue { get; set; }
    }
}
