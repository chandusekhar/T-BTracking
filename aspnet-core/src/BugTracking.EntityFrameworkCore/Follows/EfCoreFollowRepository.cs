using BugTracking.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace BugTracking.Follows
{
    public class EfCoreFollowRepository: EfCoreRepository<BugTrackingDbContext, Follow, Guid>, IFollowRepository
    {
        public EfCoreFollowRepository(
           IDbContextProvider<BugTrackingDbContext> dbContextProvider)
           : base(dbContextProvider)
        {
        }
        public async Task<List<Follow>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
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
