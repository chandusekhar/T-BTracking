using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Statuss
{
    public interface IStatusRepository : IRepository<Status, Guid>
    {
        Task<Status> FindByNameAsync(string name);

        Task<List<Status>> GetListAsync(
            string sorting,
            int skipCount,
            int maxResultCount,
            string filter = null
        );
    }
}
