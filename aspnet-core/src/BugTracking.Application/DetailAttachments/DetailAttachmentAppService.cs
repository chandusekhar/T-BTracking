using BugTracking.Attachments;
using BugTracking.DetailAttachment;
using BugTracking.IShareDto;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.DetailAttachments
{
    [Authorize("BugTracking.Users")]
    public class DetailAttachmentAppService : BugTrackingAppService, IDetailAttachmentsAppService, IRepositoryDeleteGetByIdShare<DetailAttachmentDto, Guid>
    {
        private readonly IDetailAttachmentRepository _detailAttachmentsRepository;
        private readonly DetailAttachmentManager _detailAttManager;
        private readonly IRepository<Attachment, Guid> _attRepository;
        public DetailAttachmentAppService(
            IDetailAttachmentRepository detailAttachmentsRepository, 
            DetailAttachmentManager detailAttManager, 
            IRepository<Attachment, Guid> attRepository)
        {
            _detailAttachmentsRepository = detailAttachmentsRepository;
            _detailAttManager = detailAttManager;
            _attRepository = attRepository;
        }

        public async Task<DetailAttachmentDto> GetByIdAsync(Guid Id)
        {
            var detailAtt = await _detailAttachmentsRepository.GetAsync(Id);
            return ObjectMapper.Map<DetailAttachment, DetailAttachmentDto>(detailAtt);
        }

        public async Task<ListResultDto<DetailAttachmentDto>> GetListAsync(GetListDto input)
        {
            var query = from detailAtt in _detailAttachmentsRepository
                        join att in _attRepository on detailAtt.AttachmentID equals att.Id into termAtt
                        from a in termAtt.DefaultIfEmpty()
                        orderby input.Sorting
                        select new { detailAtt, a };

            var queryResult = await AsyncExecuter.ToListAsync(query);

            var detailAttDtos = queryResult.Select(x =>
            {
                var detailAttDto = ObjectMapper.Map<DetailAttachment, DetailAttachmentDto>(x.detailAtt);
                return detailAttDto;
            }).ToList();
            var totalCount = await _detailAttachmentsRepository.GetCountAsync();
            return new PagedResultDto<DetailAttachmentDto>(
                totalCount,
                detailAttDtos
            );
        }

        public async Task<ListResultDto<DetailAttachmentDto>> GetListDetailAttByAttId(Guid attId)
        {
            var query = from detailAtt in _detailAttachmentsRepository
                        join att in _attRepository on detailAtt.AttachmentID equals att.Id into termAtt
                        from a in termAtt.DefaultIfEmpty()
                        where detailAtt.AttachmentID== attId
                        select new { detailAtt,  a  };
            var queryResult = await AsyncExecuter.ToListAsync(query);
            var detialAttDtos = queryResult.Select(x =>
            {
                var etialAttDto = ObjectMapper.Map<DetailAttachment, DetailAttachmentDto>(x.detailAtt);
                return etialAttDto;
            }).ToList();
            var totalCount = _detailAttachmentsRepository.Count(x => x.AttachmentID == attId);

            return new PagedResultDto<DetailAttachmentDto>(
                totalCount,
                detialAttDtos
                );
        }

        
        public async Task DeleteByIdAsync(Guid Id)
        {
            try
            {
                var detailAtt = await _detailAttachmentsRepository.GetAsync(Id);
                if (detailAtt == null)

                {
                    throw new EntityNotFoundException(typeof(DetailAttachment), Id);
                }
                else
                {

                    await _detailAttachmentsRepository.DeleteAsync(detailAtt);

                }
            }
            catch
            {
                throw new UserFriendlyException("An error throw while try to delete!");
            }
        }

  

        public async Task UpdateAsync(Guid id, UpdateDetailAttachmentDto input)
        {
            try
            {
                var detailAtt = await _detailAttachmentsRepository.GetAsync(id);
                if (detailAtt == null)

                {
                    throw new EntityNotFoundException(typeof(DetailAttachment), id);
                }
                else
                {
                    detailAtt.Type = input.Type;
                    detailAtt.Size = input.Size;
                    detailAtt.FileName = input.Name;

                    await _detailAttachmentsRepository.UpdateAsync(detailAtt);

                }
            }
            catch
            {
                throw new UserFriendlyException("An error throw while try to update!");
            }
        }
        public async Task CreateAsync(CreateDetailAttachmentDto input)
        {
            var detailAtt = _detailAttManager.CreateAsync(
                input.AttachmentID,
                input.Type,
                input.Size,
                input.Name
                );
            await _detailAttachmentsRepository.InsertAsync(detailAtt);
        }
    }
}
