using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;

namespace BugTracking.TimeOnProjects
{
    public class TimeOnProject : TMTAuditedAggregateRoot<Guid>
    {
        public string ApplicationName { get; set; }
        public string UrlPath { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartTime{ get; set; }
        public DateTime EndTime { get; set; }
        public string UniqueName { get; set; }
        internal TimeOnProject(
            Guid id,
            string applicationName,
            string urlPath,
            bool isActive,
            DateTime startTime,
            DateTime endTime,
            string uniqueName

            )
            : base(id)
        {
            ApplicationName = applicationName;
            UrlPath = urlPath;
            IsActive = isActive;
            StartTime = startTime;
            EndTime = endTime;
            UniqueName = uniqueName;
        }

    }
}
