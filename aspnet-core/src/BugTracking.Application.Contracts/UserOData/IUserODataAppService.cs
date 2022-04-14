using BugTracking.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BugTracking.UserOData
{
    public interface IUserODataAppService :IApplicationService
    {
        Task<IQueryable<UserDto>> GetListUserByProject(Guid idProject);
    }
}
