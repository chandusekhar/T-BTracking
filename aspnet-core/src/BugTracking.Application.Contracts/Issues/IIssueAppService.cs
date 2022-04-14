using BugTracking.Assignees;
using BugTracking.IShareDto;
using BugTracking.LevelEnum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BugTracking.Issues
{
    public interface IIssueAppService : IApplicationService
    {
        Task<PagedResultDto<IssuesDto>> GetListAsync(GetListDto input);
        Task<ListResultDto<CategoryLookupDto>> GetCategoryLookupAsync();
        Task<ListResultDto<UserLookupDto>> GetUserLookupAsync();
        List<object> GetEnumPriorityValue();
        List<object> GetEnumLevelValue();
        Task UpdateFinishDate(Guid ID, DateTime date);
        Task<PagedResultDto<IssuesDto>> GetListIssueCreatedByMe(string Filter, string statusId, string projectId,bool IsAss, int take, int skip);
        Task<ListResultDto<ProjectLookupDto>> GetProjectLookupAsync();
        Task<ListResultDto<StatusLookupDto>> GetStatusLookupAsync();
        Task UpdateAllByList(UpdateBoardDto data, Guid IdIssue);
        Task<PagedResultDto<IssuesDto>> GetListIssueByIdProject(GetListDto input, Guid IdProject, string IdCategory, string StatusName, string createrId, string assigneeId);
        Task UpdateDueDate(Guid ID, DateTime? date);
        Task<PagedResultDto<IssuesDto>> GetListCateByIdProject(Guid IdProject);
        Task<IssuesDto> UpdateCategory(Guid ID, Guid category);
        Task DeleteByIdAsync(Guid Id);
    }
}
