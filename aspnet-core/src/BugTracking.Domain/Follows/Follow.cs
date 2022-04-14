using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace BugTracking.Follows
{
    public class Follow : TMTAuditedAggregateRoot<Guid>, ISoftDelete
    {
        public Guid IssueID { get; set; }
        public string UserID { get; set; }
        public bool IsDeleted { get; set; }
        private Follow()
        {
            /* This constructor is for deserialization / ORM purpose */
        }
        internal Follow(
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
