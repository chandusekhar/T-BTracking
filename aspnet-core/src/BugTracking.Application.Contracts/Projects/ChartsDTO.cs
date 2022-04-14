using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Projects
{
    public class ChartsDTO
    {
        public DateTime date { get; set; }
        public int countIssueInDate{get; set;}
        public int countIssueCreat { get; set; }
        public int countIssueFinish { get; set; }
    }
}
