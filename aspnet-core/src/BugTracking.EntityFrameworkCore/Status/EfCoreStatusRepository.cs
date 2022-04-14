using BugTracking.EntityFrameworkCore;
using BugTracking.Statuss;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace BugTracking.Status
{

    public class EfCoreStatusRepository : EfCoreRepository<BugTrackingDbContext, Statuss.Status, Guid>, IStatusRepository
    {
        public EfCoreStatusRepository(
               IDbContextProvider<BugTrackingDbContext> dbContextProvider)
               : base(dbContextProvider)
        {
        }
        public async Task<Statuss.Status> FindByNameAsync(string name)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(status => status.Name == name);
        }

        public async Task<List<Statuss.Status>> GetListAsync(string sorting, int skipCount, int maxResultCount, string filter = null)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .WhereIf(
                    !filter.IsNullOrWhiteSpace(),
                    status => status.Name.Contains(filter)
                 )
                .Skip(skipCount)
                .Take(maxResultCount)
                .OrderBy(sorting)
                .ToListAsync();
        }
    }
}
