using BugTracking.Issues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Statuss
{
    public class StatusDTO 
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        public List<IssuesDto> IssueList{ get;  set; }
        public int CountIssue { get; set; }
        public int CountIssuePercent { get; set; }
        public int CurrentIndex { get; set; }
        public bool IsDefault { get; set; }
        public string NzColor { get; set; }
    }
}
