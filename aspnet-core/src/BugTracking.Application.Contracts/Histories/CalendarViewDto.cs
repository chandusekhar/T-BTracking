using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Histories
{
    public class CalendarViewDto
    {
        public string title { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public bool allDay { get; set; }
        public string color { get; set; }
        public Guid IssueId { get; set; }

    }
}
