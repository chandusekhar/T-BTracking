using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BugTracking.EntityFrameworkCore;

namespace BugTracking.Comments
{
    public class EfCoreCommentRepository : EfCoreRepository<BugTrackingDbContext, Comment, Guid>, ICommentRepository
    {
        public EfCoreCommentRepository(
           IDbContextProvider<BugTrackingDbContext> dbContextProvider)
           : base(dbContextProvider)
        {
        }
        public async Task<List<Comment>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
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
