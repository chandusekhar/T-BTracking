using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Departments
{
    public class UpdateDepartmentDto
    {

        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        public string IdManager { get; set; }

    }
}
