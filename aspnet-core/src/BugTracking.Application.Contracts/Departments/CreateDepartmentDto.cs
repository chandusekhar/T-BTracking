using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Departments
{
    public class CreateDepartmentDto
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        [Required]
        public string IdManager { get; set; }
    }
}
