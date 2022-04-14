using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Projects
{
    public class CountDTO
    {
        public string Date { get; set; }
        public List<ChartsDTO> result { get; set; }
    }
}
