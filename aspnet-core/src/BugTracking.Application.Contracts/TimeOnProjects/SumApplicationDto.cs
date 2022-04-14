using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.TimeOnProjects
{
    public class SumApplicationDto
    {
        public double SumTime { get; set; }
        public List<ApplicationDto> App { get; set; }
    }
}
