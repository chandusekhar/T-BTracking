using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.TimeOnProjects
{
    public class ApplicationDto
    {
        public string Name { get; set; } 
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan TotalTimeOn { get; set; }
    }
}
