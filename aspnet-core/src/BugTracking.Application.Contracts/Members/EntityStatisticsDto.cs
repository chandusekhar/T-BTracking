using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Members
{
    public class EntityStatisticsDto
    {
        public string Type { get; set; }
        public string Entity { get; set; }
        public string By { get; set; }
        public dynamic Time { get; set; }
        public Guid Id { get; set; }

    }
}
