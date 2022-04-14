using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp.Domain.Entities.Auditing;

namespace BugTracking.Members
{
    public class Member : TMTAuditedAggregateRoot<Guid>
    {
        public string UserID { get; set; }
        public Guid ProjectID { get; set; }
        private Member()
        {

        }
        internal Member(
            Guid id,
            [NotNull] Guid projectID,
            [NotNull] string userID) : base(id)
        {
            UserID = userID;
            ProjectID = projectID;
        }
    }
}
