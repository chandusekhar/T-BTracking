using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BugTracking.UserInforTfs
{
    public interface IUserInforTfsAppService : IApplicationService
    {
        Task<UserInforTfsDto> CreateAsync(CreateUserInforTfsDto createUserInforTfsDto);
        Task<UserInforTfsDto> UpdateAsync(CreateUserInforTfsDto createUserInforTfsDto);
    }
}
