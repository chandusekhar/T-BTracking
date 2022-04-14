using BugTracking.IShareDto;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace BugTracking.ServiceShare
{
    [RemoteService(IsEnabled = false)]
    public class ServiceShare : BugTrackingAppService, IGetListShareFilter<Entity, GetListDto>
    {
        public Task<PagedResultDto<Entity>> GetAllListAsync(GetListDto entity)
        {
            throw new NotImplementedException();
        }
    }

}
