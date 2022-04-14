using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;

namespace BugTracking.MemberTeams
{
    public class MemberTeam : TMTAuditedAggregateRoot<Guid>
    {
        public string UserID { get; set; }
        public Guid IdTeam { get; set; }
        private MemberTeam()
        {

        }
        internal MemberTeam(
            Guid id,
            [NotNull] Guid idTeam,
            [NotNull] string userID) : base(id)
        {
            UserID = userID;
            IdTeam = idTeam;
        }
    }
}
