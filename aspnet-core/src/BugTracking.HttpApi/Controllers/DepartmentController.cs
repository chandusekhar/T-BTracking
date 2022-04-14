using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using BugTracking.Departments;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Microsoft.TeamFoundation.Build.WebApi;
using BugTracking.Issues;
using BugTracking.IShareDto;

namespace BugTracking.Controllers
{
    [RemoteService]
    [Controller]
    [Area("department")]
    [Route("api/department")]
    public class DepartmentController : AbpController
    {
        private readonly IDepartmentAppService _departmentAppService;

        public DepartmentController(
            IDepartmentAppService departmentAppService
            )
        {
            _departmentAppService = departmentAppService;
        }
        [HttpPost("create")]
        public async  Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
        {
            return  await _departmentAppService.CreateAsync(input);
        }
        [HttpPut("edit")]
        public Task UpdateAsync(Guid id, UpdateDepartmentDto input)
        {
            return _departmentAppService.UpdateAsync(id, input);
        }
        [HttpGet("get-list")]
        public Task<ListResultDto<DepartmentDto>> GetListAsync()
        {
            return _departmentAppService.GetListAsync();
        }
        [HttpGet("get-list-issue-by-id")]
        public Task<List<IssuesDto>> GetListIssueById(Guid IdDepartment)
        {
            return _departmentAppService.GetListIssueById(IdDepartment);
        }
        [HttpGet("get-list-department")]
        public Task<List<DepartmentDto>> GetListDepartment(string filter)
        {
            return _departmentAppService.GetListDepartment(filter);
        }
        [HttpDelete("delete")]
        public Task DeleteAsync(Guid id)
        {
            return _departmentAppService.DeleteAsync(id);
        }
        [HttpGet("check-manager")]
        public bool GetCheckManager(string UserId)
        {
            return _departmentAppService.GetCheckManager(UserId);
        }
        
        /// <summary>
        /// Lấy danh sách issue 
        /// </summary>
        /// <param name="input">-HHHHHH</param>
        /// <param name="IdProject">sadsadsad</param>
        /// <param name="IdStatus">aaaaaaaa</param>
        /// <param name="IdCate"></param>
        /// <param name="IdUser"></param>
        /// <param name="IdDepartment"></param>
        /// <param name="IdTeam"></param>
        /// <param name="IsAss">assign</param>
        /// <returns></returns>
        [HttpGet("get-list-issue-all")]
        public Task<PagedResultDto<IssuesDto>> GetListIssueAll(GetListDto input, string IdProject, string IdStatus, string IdCate, string IdUser, string IdDepartment, string IdTeam, bool IsAss)
        {
            return  _departmentAppService.GetListIssueAll(input, IdProject, IdStatus, IdCate, IdUser, IdDepartment, IdTeam, IsAss);
        }
        [HttpGet("get-NameDepartment-by-idManager")]
        public List<string> getNameDepartmentByManager(string IdUser)
        {
            return _departmentAppService.getNameDepartmentByManager(IdUser);
        }
    }

}
