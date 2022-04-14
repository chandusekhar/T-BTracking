using BugTracking.Categories;
using BugTracking.ConditionTypeWit;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BugTracking.Projects
{
    public interface IProjectAppService : IApplicationService
    {
        Task<ProjectDto> GetAsyncByID(Guid id);
        Task<ProjectDto> CreateAsync(CreateProjectDto input);
        Task<ListResultDto<ProjectDto>> GetListAsync();
        Task UpdateAsync(Guid id, UpdateProjectDto input);
        Task DeleteAsync(Guid id);
        Task<ListResultDto<ProjectDto>> GetListAsyncByUserId();
        Task<List<ProjectDto>> GetListProjectByUserId(string userId,string filter);
        ItemProjectDTO GetItemProject(Guid idProject);
        IEnumerable<ReturnUpdateDto> ChartIssue(Guid IdProject);
        Dictionary<string, List<ExportFileDTO>> ExportFile();
        Task<ProjectStatisticDto> ProjectStatisticAsync(string name, string CurrentUserId);
        Task<CategoryStatisticDto> GetCategoryStatisticAsync(Guid id);
        List<string> GetCheckName(string name);
        Task<Project1StatisticDto> ProjectStatisticAsync(Guid Id, int lastDay);
        Task<ProjectTfsDto> GetProjectTfsDto(Guid Id);
        List<UserProcessingDto> GetListUserProcessing();
        List<UserProcessingDetailDto> GetUserProcessingDetails(string userId);
        Task<List<WorkItem>> GetWitsByWiql(string url, string pAT, string project, string title, string type, string state, ConditionType conditionType);

    }
}
