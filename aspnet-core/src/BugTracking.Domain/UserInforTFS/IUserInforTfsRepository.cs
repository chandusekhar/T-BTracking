using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.UserInforTFS
{
    public interface IUserInforTfsRepository : IRepository<UserInforTfs, Guid>
    {
        Task<UserInforTfs> FindByNameAsync(string userId, string uniqueName);
    }
}
