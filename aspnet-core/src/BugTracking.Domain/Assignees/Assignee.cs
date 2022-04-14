using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace BugTracking.Assignees
{
    public class Assignee : TMTAuditedAggregateRoot<Guid>
    {
        public Guid IssueID { get; set; }
        public string UserID { get; set; }
        private Assignee()
        {
            /* This constructor is for deserialization / ORM purpose */
        }
        internal Assignee(
                Guid id,
                [NotNull] Guid issueID,
                [NotNull] string userID)
            : base(id)
        {
            IssueID = issueID;
            UserID = userID;
        }
    }
}
