using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Statuss
{
    public class UpdateStatusDTO
    {
        [Required]
        public string Name { get; set; }
        public string NzColor { get; set; }
        //  public int CurrentIndex { get; set; }
    }
}
