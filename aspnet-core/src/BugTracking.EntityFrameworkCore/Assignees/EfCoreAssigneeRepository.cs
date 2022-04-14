using BugTracking.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BugTracking.Assignees
{
    public class EfCoreAssigneeRepository : EfCoreRepository<BugTrackingDbContext, Assignee, Guid>, IAssigneeRepository
    {
        public EfCoreAssigneeRepository(
           IDbContextProvider<BugTrackingDbContext> dbContextProvider)
           : base(dbContextProvider)
        {
        }
        public async Task<List<Assignee>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }
    }
}
