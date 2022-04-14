using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.DepartmentOData
{
   public class DepartmentODataDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IdManager { get; set; }
        public string NameManager { get; set; }
        public int CountMember { get; set; }
        public int CountIssue { get; set; }
    }
}
