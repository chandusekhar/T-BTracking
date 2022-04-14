using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BugTracking.Follows
{
    public interface IFollowService: IApplicationService
    {
        //Task<PagedResultDto<FollowDto>> GetListAsyncByIdUser(string IdUser, Guid IdProject);
        Task<FollowDto> GetByIdAsync(Guid id);
        //Task CreateByListAsync(string[] followList, Guid idIssue);
        Task<FollowDto> CreateAsync(CreateFollowDto input);
        Task DeleteAsync(Guid id);
    }
}
