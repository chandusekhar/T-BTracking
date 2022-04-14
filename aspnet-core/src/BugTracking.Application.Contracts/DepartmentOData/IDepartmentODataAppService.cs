using BugTracking.Departments;
using BugTracking.MemberTeams;
using BugTracking.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BugTracking.DepartmentOData
{
    public interface IDepartmentODataAppService : IApplicationService
    {
        Task<IQueryable<DepartmentODataDto>> GetListDepartment();
        Task<IQueryable<TeamDto>> GetListTeam();
        Task<IQueryable<MemberTeamDto>> GetListMemberByTeam();
    }
}
