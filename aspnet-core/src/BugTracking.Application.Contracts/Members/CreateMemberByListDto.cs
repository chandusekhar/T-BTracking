using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Members
{
    public class CreateMemberByListDto
    {
        [Required]
        public Guid ProjectID { get; set; }
        [Required]
        public string[] UserIDs { get; set; }
    }
}
