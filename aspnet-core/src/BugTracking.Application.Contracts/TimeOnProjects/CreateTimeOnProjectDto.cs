using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.TimeOnProjects
{
    public class CreateTimeOnProjectDto
    {
        public string ApplicationName { get; set; }
        public string UrlPath { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string UniqueName { get; set; }
    }
}
