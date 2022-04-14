using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp.Domain.Entities.Auditing;

namespace BugTracking.Histories
{
    public class History : TMTAuditedAggregateRoot<Guid>
    {
        public string ApplicationName { get; set; }

        public string AuditLogId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Entity { get; set; }
        public string InProject { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime ExcutitonTime { get; set; }
        public int Count { get; set; }
        private History()
        {

        }

        internal History(
            Guid id,
            string applicationName,
            string auditLogId,
            string userName,
            string action,
            string entity,
            string inProject,
            string oldValue,
            string newValue,
            DateTime excutitonTime,
            int count
    )
    : base(id)
        {
            ApplicationName = applicationName;
            AuditLogId = auditLogId;
            UserName = userName;
            Action = action;
            Entity = entity;
            InProject = inProject;
            OldValue = oldValue;
            NewValue = newValue;
            ExcutitonTime = excutitonTime;
            Count = count;
        }
    }
}
