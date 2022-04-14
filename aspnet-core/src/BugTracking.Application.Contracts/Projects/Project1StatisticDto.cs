using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Projects
{
    public class Project1StatisticDto
    {
        public int Created { get; set; }
        public int Closed { get; set; }
        public decimal pCreated { get; set; }
        public decimal pClosed { get; set; }
        public List<Member1StatisticDto> member1StatisticDtos { get; set; }

    }
}
