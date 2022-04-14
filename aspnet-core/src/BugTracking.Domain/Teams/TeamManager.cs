using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace BugTracking.Teams
{
    public class TeamManager : DomainService
    {
        private readonly ITeamRepository _teamRepository;
        public TeamManager(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }
        public Team CreateAsync(
         [NotNull] string name,
          string idLeader,
          Guid idDepartment
          )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));


            return new Team(
                GuidGenerator.Create(),
                name,
                idLeader,
                idDepartment
            );
        }
    }
}
