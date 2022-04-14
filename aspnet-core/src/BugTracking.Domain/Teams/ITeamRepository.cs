using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Teams
{
    public interface ITeamRepository : IRepository<Team, Guid>
    {
        Task<List<Team>> GetListAsync(
         int skipCount,
         int maxResultCount,
         string sorting,
         string filter = null
     );
    }
}
