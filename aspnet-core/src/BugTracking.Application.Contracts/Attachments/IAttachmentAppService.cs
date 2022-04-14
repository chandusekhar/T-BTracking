using BugTracking.DetailAttachment;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BugTracking.Attachments
{
    public interface IAttachmentAppService : IApplicationService
    {
        Task<PagedResultDto<AttachmentDto>> GetListAsync(GetAttachmentListDto input);
        Task<AttachmentDto> GetAsync(Guid id);
        //Task<AttachmentDto>  CreateAsync(CreateAttachmentDto input);

        //Task UpdateAsync(List<CreateDetailAttachmentDto> attachmentDtos, Guid ID);
        Task CreateByListAttachmentAsync(List<CreateDetailAttachmentDto> createDetailAttachmentDto,Guid ID);
        //Task CreateListAsync(List<CreateAttachmentDto> list);
    }
}
