using BugTracking.Admin;
using BugTracking.AdminOData;
using BugTracking.Issues;
using BugTracking.Projects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Uow;

namespace BugTracking.Controllers
{
    [RemoteService]
    //[Controller]
    // [Microsoft.AspNetCore.Mvc.Route("admin")]
    [IgnoreAntiforgeryToken]
    [ODataAttributeRouting]
    // [EnableQuery]
      [UnitOfWork]
    
    public class AdminODataController : ODataController
    {
        private readonly IAdminODataAppService _adminODataAppService;
        public AdminODataController(IAdminODataAppService adminODataAppService)
        {
            _adminODataAppService = adminODataAppService;
        }
        /// <summary>
        /// Danh sách user form admin
        /// </summary>
        /// <returns></returns>
        [EnableQuery]
        public Task<IQueryable<AdminODataDto>> Get()
        {
            return _adminODataAppService.GetUserAdmin();
        }
        /// <summary>
        /// Danh sách project form admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery]
        public async Task<IQueryable<ProjectDto>> getListProject()
        {
            return await _adminODataAppService.GetListProject();
        }
        /// <summary>
        /// Danh sách issue cho form admin
        /// </summary>
        /// <param name="UserId">User assign</param>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery]
        public async Task<IQueryable<IssueAdminODataDto>> GetListIssue(string UserId)
        {
            return await _adminODataAppService.GetListIssue(UserId);
        }
        [EnableQuery]
        public Task<IQueryable<AdminODataDto>> Delete(int key)
        {
            return _adminODataAppService.GetUserAdmin();
        }
    }
}
