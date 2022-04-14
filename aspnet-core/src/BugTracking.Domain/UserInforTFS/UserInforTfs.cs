using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;

namespace BugTracking.UserInforTFS
{
    public class UserInforTfs : TMTAuditedAggregateRoot<Guid>
    {
        public string UserId { get; set; }
        public string UniqueName { get; set; }
        public string PAT { get; set; }
        internal UserInforTfs(
            Guid id,
            [NotNull] string userId,
            [NotNull] string uniqueName,
            [NotNull] string pAT

            )
            : base(id)
        {
            UserId = userId;
            UniqueName = uniqueName;
            PAT = pAT;
        }
    }
}
