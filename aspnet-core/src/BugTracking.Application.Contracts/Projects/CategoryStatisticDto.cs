using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Projects
{
    public class CategoryStatisticDto
    {
        public string[] Categories { get; set; }
        public int[] Issues { get; set; }
    }
}
