using BugTracking.DetailAttachment;
using BugTracking.DetailAttachments;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Attachments
{
    //[Authorize("BugTracking.Users")]
    public class AttachmentAppService : BugTrackingAppService, IAttachmentAppService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly AttachmentManager _attachmentManager;
        private readonly IDetailAttachmentRepository _detailAttRepository;
        private readonly DetailAttachmentManager _DetailAttachmentManager;
        private readonly IConfiguration _configuration;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        [Obsolete]
        private readonly IHostingEnvironment _environment;
        private static readonly List<string> ListFileUpload=new List<string>();
        
        [Obsolete]
        public AttachmentAppService(
            IAttachmentRepository attachmentRepository,
            AttachmentManager attachmentManager,
            DetailAttachmentManager DetailAttachmentManager,
            IHostingEnvironment environment,
            IDetailAttachmentRepository detailAttRepository,
            IConfiguration configuration,
            ITMTCurrentUser tmtCurrentUser)
        {
            _attachmentRepository = attachmentRepository;
            _attachmentManager = attachmentManager;
            _environment = environment;
            _detailAttRepository = detailAttRepository;
            _DetailAttachmentManager = DetailAttachmentManager;
            _configuration = configuration;
            _tmtCurrentUser = tmtCurrentUser;
        }
        public async Task CreateByListAsync(string[] attList, Guid commentId)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            foreach (string att in attList)
            {

                var attachment = _attachmentManager.CreateAsync(att, commentId);
                attachment.CreatorId = CurrentUserId;
                await _attachmentRepository.InsertAsync(attachment);
            }
        }
        public void RemoveListFile(string fileName)
        {
            if (!fileName.IsNullOrWhiteSpace())
            {
                ListFileUpload.Remove(ListFileUpload.Where(x=>x.Contains(fileName)).FirstOrDefault());
            }
        }

        [Obsolete]
        public void CreateAttachment([FromForm] CreateAttachmentDto input)
        {
            //var CurrentUserId = _tmtCurrentUser.GetId();
            string uploads = Path.Combine(_environment.WebRootPath, "attachments");
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }
            var generateFileName = Path.GetRandomFileName() + input.File.FileName;
            var filePath = Path.Combine(uploads, generateFileName);
            //100MB
            if (input.File.Length > 0 && 
                input.File.Length <= long.Parse(_configuration.GetSection("MaxSizeAttachment")["Size"]))
            {
                using var fileStream = new FileStream(filePath, FileMode.Create);
                input.File.CopyTo(fileStream);
                ListFileUpload.Add("/attachments/" + generateFileName);
            }
        }
        public async Task CreateByListAttachmentAsync(List<CreateDetailAttachmentDto> createDetailAttachmentDto,Guid ID)
        {
            //var CurrentUserId = _tmtCurrentUser.GetId();
                for (var i = 0; i < createDetailAttachmentDto.Count; i++)
            {
                var FileName = 
                    ListFileUpload.Where(x => x.Contains(createDetailAttachmentDto[i].Name)).FirstOrDefault();
                var attachment = _attachmentManager.
                    CreateAsync(FileName, ID);

                    var AttachmentID = await _attachmentRepository.InsertAsync(attachment);
                    var detailAttachment = _DetailAttachmentManager.CreateAsync(
                    AttachmentID.Id,
                    createDetailAttachmentDto[i].Type,
                    createDetailAttachmentDto[i].Size,
                    createDetailAttachmentDto[i].Name

                );
                await _detailAttRepository.InsertAsync(detailAttachment);
                ListFileUpload.RemoveAll(x => x.Contains(FileName));
                }
        }
        public async Task DeleteAsync(Guid id)
        {

            try
            {
                var attachment = await _attachmentRepository.GetAsync(id);
                if (attachment == null)

                {
                    throw new EntityNotFoundException(typeof(Attachment), id);
                }
                else
                {
                    var ListDetailAttachment = await _detailAttRepository.
                        GetListAsync(x => x.AttachmentID == attachment.Id);
                    if (ListDetailAttachment.Any())
                    {
                        await _detailAttRepository.DeleteManyAsync(ListDetailAttachment);
                    }
                    await _attachmentRepository.DeleteAsync(attachment);
                }
            }
            catch
            {
                throw new UserFriendlyException("An error while try to delete attachment !!");
            }
        }

        public async Task<AttachmentDto> GetAsync(Guid id)
        {
            var queryable = await _attachmentRepository.GetQueryableAsync();
            var query = from att in queryable
                        where att.TableID == id
                        select new
                        {
                            att
                        };
            var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);
            if (queryResult == null)
            {
                throw new EntityNotFoundException(typeof(Attachment), id);
            }

            var attDto = ObjectMapper.Map<Attachment, AttachmentDto>(queryResult.att);
            return attDto;
        }
        public async Task<PagedResultDto<AttachmentDto>> GetListAsync(GetAttachmentListDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = nameof(Attachment.CreationTime);
            }
            var attachment = await _attachmentRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting,
                input.Filter
            );

            var totalCount = await _attachmentRepository.CountAsync();

            return new PagedResultDto<AttachmentDto>(
                totalCount,
                ObjectMapper.Map<List<Attachment>, List<AttachmentDto>>(attachment)
            );
        }

        public async Task UpdateAsync(List<AttachmentDto> attachmentDtos,Guid ID)
        {
            try
            {
                //var CurrentUserId = _tmtCurrentUser.GetId();
                var ListAttachmentDB = await _attachmentRepository.GetListAsync(x => x.TableID == ID);
                for (var i = 0; i < attachmentDtos.Count; i++)
                {
                    for (var j = 0; j < ListAttachmentDB.Count; j++)
                    {
                        if (ListAttachmentDB[j].Id == attachmentDtos[i].Id)
                        {
                            ListAttachmentDB.RemoveAll(x => x.Id == ListAttachmentDB[j].Id);
                        }
                    }
                }
                foreach (Attachment attachment in ListAttachmentDB)
                {
                    await DeleteAsync(attachment.Id);
                }
                var listAddAttachment = await _attachmentRepository.GetListAsync(x => x.TableID == ID);
                for (var i = 0; i < listAddAttachment.Count; i++)
                {
                    for (var j = 0; j < attachmentDtos.Count; j++)
                    {
                        if (attachmentDtos[j].Id == listAddAttachment[i].Id)
                        {
                            attachmentDtos.RemoveAll(x => x.Id == listAddAttachment[i].Id);
                        }
                    }
                }
                foreach (AttachmentDto attachmentDto in attachmentDtos)
                {
                    var FileName = ListFileUpload.
                                    Where(x => x.Contains(attachmentDto.Name)).FirstOrDefault();
                    var attachment = _attachmentManager.CreateAsync(FileName, ID);
                    var AttachmentID = await _attachmentRepository.InsertAsync(attachment);
                    var detailAttachment = _DetailAttachmentManager.CreateAsync(
                    AttachmentID.Id,
                    attachmentDto.Type,
                    attachmentDto.Size,
                    attachmentDto.Name
                );
                    await _detailAttRepository.InsertAsync(detailAttachment);
                    ListFileUpload.RemoveAll(x => x.Contains(FileName));
                }
            }
            catch { throw new UserFriendlyException("An error while try to upload multi file!!!"); }

        }
    }
}
