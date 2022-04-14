using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Projects
{
    public class ReturnUpdateDto
    {
        public string Date { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public object Data { get; set; }
        public int CountCreate { get; set; }
        public int CountFinish { get; set; }
        //public int TotalCreate { get; set; }
        //public int TotalFinish { get; set; }
    }
}
