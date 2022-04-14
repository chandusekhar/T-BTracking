using System;

namespace BugTracking.Dashboards
{
    public class DashboardDto
    {
        public class DBUpdateDto
        {
            public string UserName { get; set; }
            public string AuditLogId { get; set; }
            public object UserInfo { get; set; }
            public object Action { get; set; }
            public string Entity { get; set; }
            public string InProject { get; set; }
            public string OldValue { get; set; }
            public string NewValue { get; set; }
            public DateTime ExcutitonTime { get; set; }
        };

        public class ReturnUpdatesDto
        {
            public string Date { get; set; }
            public object Data { get; set; }
        }

        public class ReturnChartDto
        {
            public string Year { get; set; }
            public object Data { get; set; }
        }

        public class ChartIssueDto
        {
            public string StatusName { get; set; }
            public DateTime CreationTime { get; set; }
        };
    }
}