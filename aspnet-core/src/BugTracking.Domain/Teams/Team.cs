using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp;

namespace BugTracking.Teams
{
    public class Team : TMTAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string IdLeader { get; set; }
        public Guid IdDepartment { get; set; }

        private Team()
        {

        }
        internal Team(
            Guid id,
            [NotNull] string name,
            string idLeader,
            Guid idDeparntment

            )
            : base(id)
        {
            SetName(name);
            IdLeader = idLeader;
            IdDepartment = idDeparntment;
        }
        internal Team ChangeName([NotNull] string teamName)
        {
            SetName(teamName);
            return this;
        }

        private void SetName([NotNull] string teamName)
        {
            Name = Check.NotNullOrWhiteSpace(
                teamName,
                nameof(teamName)
            );
        }
    }
}
