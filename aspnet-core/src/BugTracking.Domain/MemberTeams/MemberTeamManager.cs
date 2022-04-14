using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace BugTracking.MemberTeams
{
    public class MemberTeamManager : DomainService
    {
        private readonly IMemberTeamRepository _memberTeamRepository;
        public MemberTeamManager(IMemberTeamRepository memberTeamRepository)
        {
            _memberTeamRepository = memberTeamRepository;
        }
        public async Task<MemberTeam> CreateAsync(
            Guid idTeam,
            string userID
            )
        {
            var query = await _memberTeamRepository.FindAsync(x => x.IdTeam == idTeam && x.UserID == userID);
            return new MemberTeam(
                GuidGenerator.Create(),
                idTeam,
                userID
            );
        }
    }
}
