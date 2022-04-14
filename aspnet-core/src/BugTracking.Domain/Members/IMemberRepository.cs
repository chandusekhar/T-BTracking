using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Members
{
    public interface IMemberRepository : IRepository<Member, Guid>
    {
        Task<List<Member>> GetListAsync(
           int skipCount,
           int maxResultCount,
           string sorting,
           string filter = null
       );
    }
}
