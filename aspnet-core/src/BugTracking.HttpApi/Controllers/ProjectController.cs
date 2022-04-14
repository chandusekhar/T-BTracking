
using BugTracking.ConditionTypeWit;
using BugTracking.Issues;
using BugTracking.Projects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Controllers
{
    [RemoteService]
    [Controller]
    [Area("project")]
    [Route("api/project")]
    public class ProjectController : BugTrackingController
    {
        private readonly IProjectAppService _projectAppService;

        public ProjectController(
            IProjectAppService projectAppService
            )
        {
            _projectAppService = projectAppService;
        }
        [HttpPost("create")]
        public Task<ProjectDto> CreateAsync(CreateProjectDto input)
        {
            return _projectAppService.CreateAsync(input);
        }
        [HttpGet("get-list")]
        public Task<ListResultDto<ProjectDto>> GetListAsync()
        {
            return _projectAppService.GetListAsync();
        }
        [HttpPut("edit")]
        public Task UpdateAsync(Guid id, UpdateProjectDto input)
        {
            return _projectAppService.UpdateAsync(id, input);
        }
        [HttpDelete("delete")]
        public Task DeleteAsync(Guid id)
        {
            return _projectAppService.DeleteAsync(id);
        }
        [HttpGet("get-by-id")]
        public Task<ProjectDto> GetAsyncByID(Guid id)
        {
            return _projectAppService.GetAsyncByID(id);
        }

        [HttpGet("get-list-by-user-id")]
        public Task<ListResultDto<ProjectDto>> GetListAsyncByUserId()
        {
            return _projectAppService.GetListAsyncByUserId();
        }
        [HttpGet("get-list-project-by-user")]
        public Task<List<ProjectDto>> GetListProjectByUserId(string userId, string filter)
        {
            return _projectAppService.GetListProjectByUserId(userId, filter);
        }
        [HttpGet("get-list-Item-In-Project")]
        public  ItemProjectDTO GetItemProject(Guid idProject)
        {
            return _projectAppService.GetItemProject(idProject);
        }
        [HttpGet("get-isssue-chart")]
        public IEnumerable<ReturnUpdateDto> ChartIssue(Guid IdProject)
        {
            return  _projectAppService.ChartIssue(IdProject);
        }
        [HttpGet("exportFile")]
        public  Dictionary<string, List<ExportFileDTO>> ExportFile()
        {
            return _projectAppService.ExportFile();
        }
        [HttpGet("ProjectStatistic")]
        public Task<ProjectStatisticDto> ProjectStatistic(string name, string currentUserId)
        {
            return _projectAppService.ProjectStatisticAsync(name, currentUserId);
        }
        [HttpGet("CategoryStatistic")]
        public Task<CategoryStatisticDto> GetCategoryStatisticAsync(Guid id)
        {
            return _projectAppService.GetCategoryStatisticAsync(id);
        }
        [HttpGet("CheckNameExist")]
        public List<string> GetCheckName(string name)
        {
            return _projectAppService.GetCheckName(name);
        }
        [HttpGet("project-1-statistic")]
        public Task<Project1StatisticDto> ProjectStatisticAsync(Guid Id, int lastDay)
        {
            return _projectAppService.ProjectStatisticAsync(Id, lastDay);
        }
        [HttpGet("project-tfs-dto")]
        public Task<ProjectTfsDto> GetProjectTfsDto(Guid Id)
        {
            return _projectAppService.GetProjectTfsDto(Id);
        }
        [HttpGet("user-processing-dto")]
        public List<UserProcessingDto> GetListUserProcessing()
        {
            return _projectAppService.GetListUserProcessing();
        }
        [HttpGet("user-processing-details-dto")]
        public List<UserProcessingDetailDto> GetUserProcessingDetails(string userId)
        {
            return _projectAppService.GetUserProcessingDetails(userId);
        }
        /// <summary>
        /// Get Work Items Assigned To Me Or Follows By Me (Work Item = WIT)
        /// </summary>
        /// <param name="url"> Url Collection. Ex: https://tfs.tpos.dev/TMTInternship </param>
        /// <param name="pAT"> Personal Access Token</param>
        /// <param name="project"> Project Name. Default : Get All Project </param>
        /// <param name="title"> WIT Title (Name) </param>
        /// <param name="types"> WIT Types. Ex: Task,Bug, ...</param>
        /// <param name="states"> WIT States. Ex: New,Closed, ...</param>
        /// <param name="conditionType">0: Assigned To Me, 1: Follows By Me</param>
        /// <returns></returns>
        [HttpGet("wITs-by-wIql")]
        public Task<List<WorkItem>> GetWitsByWiql(string url, string pAT, string project, string title, string types, string states, ConditionType conditionType)
        {
            return _projectAppService.GetWitsByWiql(url, pAT, project, title, types, states, conditionType);
        }
    }
}
