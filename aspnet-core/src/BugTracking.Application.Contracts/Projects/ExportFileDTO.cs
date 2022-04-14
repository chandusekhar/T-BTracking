using BugTracking.PriorityEnum;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Projects
{
    public class ExportFileDTO
    {
        public string IssueName { get; set; }
        public string Description { get; set; } 
        public string StatusName { get; set; }
        public string CategoryName { get; set; }
        public DateTime? DueDate { get; set; }
        public string UserName { get; set; }
        public string ProjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public string PriorityValue { get; set; }
        public string LevelValue { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string CreatorId { get; set; }
    }
}
