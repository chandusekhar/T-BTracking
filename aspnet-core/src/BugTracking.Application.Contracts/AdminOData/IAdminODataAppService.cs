using BugTracking.Issues;
using BugTracking.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BugTracking.AdminOData
{
    public interface IAdminODataAppService : IApplicationService
    {
        Task<IQueryable<ProjectDto>> GetListProject();
        Task<IQueryable<AdminODataDto>> GetUserAdmin();
        Task<IQueryable<IssueAdminODataDto>> GetListIssue(string idUserAssign);
    }
}
