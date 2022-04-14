using BugTracking.DepartmentOData;
using BugTracking.Departments;
using BugTracking.MemberTeams;
using BugTracking.Teams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Uow;

namespace BugTracking.Controllers
{

    [RemoteService]
    [IgnoreAntiforgeryToken]
    [ODataAttributeRouting]
    [UnitOfWork]

    public class DepartmentODataController : ODataController
    {
        private readonly IDepartmentODataAppService _departmentODataAppService;
        // GET /User(1)/Supplier
        public DepartmentODataController(IDepartmentODataAppService departmentODataAppService)
        {
            _departmentODataAppService = departmentODataAppService;
        }
        //Task<IQueryable<DepartmentDto>> GetListDepartment();
        /// <summary>
        /// Danh sách department
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        public Task<IQueryable<DepartmentODataDto>> Get()
        {
            return _departmentODataAppService.GetListDepartment();
        }
        /// <summary>
        /// Danh sách team
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery]
        public async Task<IQueryable<TeamDto>> GetListTeam()
        {
            return await _departmentODataAppService.GetListTeam();
        }
        /// <summary>
        /// Danh sách thành viên của team
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery]
        public async Task<IQueryable<MemberTeamDto>> GetListMemberByTeam()
        {
            return await _departmentODataAppService.GetListMemberByTeam();
        }
        
    }
}
