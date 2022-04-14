using BugTracking.EntityFrameworkCore;
using BugTracking.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Volo.Abp.Identity;

namespace BugTracking.User
{
    public class EfCoreUserRepository : EfCoreRepository<BugTrackingDbContext, Users.AppUser, string>, IUserRepository
    {
        public EfCoreUserRepository(IDbContextProvider<BugTrackingDbContext> dbContextProvider)
               : base(dbContextProvider)
        {
        }
        public async Task<Users.AppUser> FindByNameAsync(string name)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(user => user.Name == name);
        }
        public async Task<Users.AppUser> FindByPhoneNumberAsync(string phoneNumber)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(user => user.PhoneNumber == phoneNumber);
        }
        public async Task<List<Users.AppUser>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .WhereIf(
                    !filter.IsNullOrWhiteSpace(),
                    user => user.Name.Contains(filter)
                 )
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }
        //public Task<string> FindRoleByUserId(string UserId)
        //{

        //    return
        //}
    }
}
