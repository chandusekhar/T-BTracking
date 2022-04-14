using BugTracking.IShareDto;
using BugTracking.Issues;
using BugTracking.Projects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using static BugTracking.Dashboards.DashboardDto;

namespace BugTracking.Dashboards
{
    public interface IDashboardService : IApplicationService
    {
        //Task<IEnumerable<ReturnUpdatesDto>> GetUpdates();

        //Task<List<GetIssueByUserDto>> GetAllIssueByCurrentUser(string param);

        //Task<IEnumerable<ReturnChartDto>> GetChart(string type);
        Task<List<ProjectDto>> GetListProject(GetListDto input);
    }
}