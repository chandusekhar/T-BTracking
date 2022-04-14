using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Follows
{
    public interface IFollowRepository : IRepository<Follow, Guid>
    {
        Task<List<Follow>> GetListAsync(
           int skipCount,
           int maxResultCount,
           string sorting,
           string filter = null
       );
    }
}
