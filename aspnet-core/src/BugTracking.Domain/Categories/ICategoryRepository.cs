using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Categories
{
    public interface ICategoryRepository : IRepository<Category, Guid>
    {
        Task<Category> FindByCategoryNameAsync(string name);
         
        Task<List<Category>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string filter = null
        );
    }
}
