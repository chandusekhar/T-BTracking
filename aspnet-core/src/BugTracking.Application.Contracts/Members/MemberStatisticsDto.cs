using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Members
{
    public class MemberStatisticsDto
    {
        public string Name { get; set; }
        public int Create { get; set; }
        public int Tag { get; set; }
        public int Close { get; set; }
        public decimal pCreate { get; set; } = 0;
        public decimal pTag { get; set; } = 0;
        public decimal pClose { get; set; } = 0;
        public List<EntityStatisticsDto> entityStatisticsDtos { get; set; }
    }
}
