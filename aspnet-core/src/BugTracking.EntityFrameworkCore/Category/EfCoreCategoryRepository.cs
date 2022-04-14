using BugTracking.Categories;
using BugTracking.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace BugTracking.Category
{
    public class EfCoreCategoryRepository : EfCoreRepository<BugTrackingDbContext, Categories.Category, Guid>, ICategoryRepository
    {
        public EfCoreCategoryRepository(
            IDbContextProvider<BugTrackingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
        public async Task<Categories.Category> FindByCategoryNameAsync(string name)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(category => category.Name == name);
        }

        public async Task<List<Categories.Category>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .WhereIf(
                    !filter.IsNullOrWhiteSpace(),
                    category => category.Name.Contains(filter)
                 )
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }
    }
}
