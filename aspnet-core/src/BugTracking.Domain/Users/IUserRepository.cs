using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Users
{
    public interface IUserRepository : IRepository<AppUser, string>
    {
        Task<AppUser> FindByPhoneNumberAsync(string phoneNumber);

        Task<AppUser> FindByNameAsync(string name);

        Task<List<AppUser>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string filter = null
        );

        //Task<string> FindRoleByUserId(string UserId);
    }
}