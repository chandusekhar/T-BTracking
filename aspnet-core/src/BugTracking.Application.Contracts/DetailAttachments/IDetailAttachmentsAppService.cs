using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using BugTracking.IShareDto;

namespace BugTracking.DetailAttachment
{
    public interface IDetailAttachmentsAppService : IApplicationService
    {
        Task<ListResultDto<DetailAttachmentDto>> GetListAsync(GetListDto input);
        Task<ListResultDto<DetailAttachmentDto>> GetListDetailAttByAttId(Guid attID);
        Task CreateAsync(CreateDetailAttachmentDto input);
        Task UpdateAsync(Guid id, UpdateDetailAttachmentDto input);
    }
}
