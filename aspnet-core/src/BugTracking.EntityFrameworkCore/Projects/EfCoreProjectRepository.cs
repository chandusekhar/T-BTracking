using BugTracking.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace BugTracking.Projects
{
    public class EfCoreProjectRepository : EfCoreRepository<BugTrackingDbContext, Project, Guid>, IProjectRepository
    {
        public EfCoreProjectRepository(
            IDbContextProvider<BugTrackingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
        public async Task<Project> FindByNameAsync(string name)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(project => project.Name == name);
        }

        public Task<List<Project>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            throw new NotImplementedException();
        }
    }
}
