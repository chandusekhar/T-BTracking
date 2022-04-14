using BugTracking.IShareDto;
using BugTracking.Statuss;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace BugTracking.Controllers
{
    [Route("api/status")]
    public class StatusController : AbpController
    {

        private readonly IStatusService _statusService;
        public StatusController(IStatusService statusService)
        {

            _statusService = statusService;
        }
        [HttpPost]
        public async Task<StatusDTO> CreateAsync(CreateStatusDTO input)
        {
            var status = await _statusService.CreateAsync(input);

            return status;
        }

        [HttpGet("GetList")]
        public async Task<List<StatusDTO>> GetListAsync()
        {
            var status = await _statusService.GetListAsync();

            return status;
        }
        [HttpGet("GetListStatus")]
        public async Task<List<StatusDTO>> GetListStatusAsync(Guid IdProject, string Filter,
                                                                string idUser, string dueDate, string idCategory, int skip, int take)
        {
            var status = await _statusService.GetListStatusAsync(IdProject, Filter, idUser, dueDate, idCategory,skip,take);

            return status;
        }
        [HttpGet("GetListCategoryBoard")]
        public async Task<List<CategoryBoardDto>> GetListCategoryAsync(Guid IdProject, string Filter,
                                                               string idUser, string dueDate, string idCategory)
        {
            var category = await _statusService.GetListCategoryAsync(IdProject, Filter, idUser, dueDate, idCategory);

            return category;
        }
        [HttpGet("Get(ID)")]
        public async Task<StatusDTO> GetAsync(Guid id)
        {
            var status = await _statusService.GetAsync(id);

            return status;
        }
        [HttpGet("CheckIssueInProject")]
        public async Task<bool> CheckIssueInProject(Guid idProject)
        {
            var status = await _statusService.CheckIssueInProject(idProject);

            return status;
        }
        [HttpDelete]
        public async Task DeleteAsync(Guid id)
        {
            await _statusService.DeleteAsync(id);
        }
        [HttpGet("Status-Statistics")]
        public async Task GetStatusStatistics(Guid projectId)
        {
            await _statusService.GetStatusStatistics(projectId);
        }

        //    [HttpPut]
        //    public async Task UpdateAsync(Guid id, UpdateStatusDTO input)
        //    {
        //        await _statusService.UpdateAsync(id , input);
        //    }
        //    [HttpGet]
        //    public async Task<Guid> GetIdbyName(string Name)
        //    {
        //        var resultName = await _statusService.GetIdbyName(Name);
        //        return resultName;
        //    }
        //[HttpPut("update-all")]
        //public async Task UpdateAllByList(List<StatusDTO> statusDtos)
        //{
        //    await _statusService.UpdateAllByList(statusDtos);
        //}
    }
}
