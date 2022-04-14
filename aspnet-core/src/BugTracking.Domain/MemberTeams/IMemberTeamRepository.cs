using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.MemberTeams
{
    public interface IMemberTeamRepository : IRepository<MemberTeam, Guid>
    {
        Task<List<MemberTeam>> GetListAsync(
           int skipCount,
           int maxResultCount,
           string sorting,
           string filter = null
       );
    }
}
