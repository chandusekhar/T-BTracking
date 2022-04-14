using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp.Domain.Entities.Auditing;

namespace BugTracking.Notifications
{
    public class Notification : TMTAuditedAggregateRoot<Guid>
    {
        public string Message { get; set; }
        public Guid IssueID { get; set; }
        public string UserID { get; set; }
        public bool IsRead { get; set; }
        private Notification()
        {

        }
        internal Notification(
            Guid id,
            [NotNull] Guid issueID,
            string userID,
            string message,
            bool isRead)
            : base(id)
        {
            IssueID = issueID;
            UserID = userID;
            Message = message;
            IsRead = isRead;
        }
    }
}
