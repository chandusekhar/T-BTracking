using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BugTracking.Azures
{
    public interface IAzureAppService : IApplicationService
    {
        Task InsertComment(Guid id, Guid IdWIT, string comment, string projectName, string CurrentUserId);
        Task DeleteComment(Guid IdWIT, string projectName, int idCommentWIT, string CurrentUserId);
        Task UpdateComment(Guid id, Guid IdWIT, string comment, string projectName, string CurrentUserId);
        Task UpdateWIT(Guid id, string name, string description, string priority, List<string> Assignees, string CurrentUserId);
        Task DeleteWIT(int id,Guid projectId, string CurrentUserId);
        Task CreateWIT(Guid id,string project, string name, string description,string priority,List<string> Assignees, string CurrentUserId,Guid issueParentId);
        Task UpdateState(Guid id, Guid statusId, string CurrentUserId);
        Task CreateProject(string name, string CurrentUserId, string des);
        Task UpdateProject(Guid id, string name, string CurrentUserId, string des);
        Task DeleteProject(Guid id, string CurrentUserId);
        Task<bool> GetProjectsFromTFS(string name, string CurrentUserId);
        Task UpdateAssignee(Guid id, List<string> Assignees,bool IsDefault, string CurrentUserId);
        Task CreateMember(Guid projectId, string accountName);
    }
}
