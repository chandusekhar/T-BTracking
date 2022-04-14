using BugTracking.EntityFrameworkCore;
using BugTracking.UserInforTFS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace BugTracking
{
    public class EFCoreUserInforTfsRepository : EfCoreRepository<BugTrackingDbContext, UserInforTFS.UserInforTfs, Guid>, IUserInforTfsRepository
    {
        public EFCoreUserInforTfsRepository(
           IDbContextProvider<BugTrackingDbContext> dbContextProvider)
           : base(dbContextProvider)
        {
        }

        public async Task<UserInforTfs> FindByNameAsync(string userId, string uniqueName)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(user => user.UserId == userId && user.UniqueName == uniqueName);
        }
    }
}
