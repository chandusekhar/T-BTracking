using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BugTracking.Admin
{
    public interface IAdminService
    {
        Task<IQueryable<Users.UserDto>> GetUserAdmin();
    }
}
