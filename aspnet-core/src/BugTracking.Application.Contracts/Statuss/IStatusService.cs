using BugTracking.IShareDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BugTracking.Statuss
{
    public interface IStatusService : IApplicationService
    {
        Task<List<StatusDTO>> GetListAsync();
        Task <StatusDTO> GetAsync(Guid id);

        Task<StatusDTO> CreateAsync(CreateStatusDTO input);
        Task<bool> CheckIssueInProject(Guid idProject);
        Task UpdateAsync(Guid id, UpdateStatusDTO input);
        Task<List<StatusDTO>> GetListOnlyAsync();
        Task DeleteAsync(Guid id);
        Task UpdateAllByList(List<StatusDTO> statusDtos);
        Task<Guid> GetIdbyName(string Name);
        Task<List<StatusDTO>> GetListStatusAsync(Guid IdProject, string Filter,
                                                                string idUser, string dueDate, string idCategory, int skip, int take);
        Task<List<CategoryBoardDto>> GetListCategoryAsync(Guid IdProject, string Filter,
                                                               string idUser, string dueDate, string idCategory);
        Task<PagedResultDto<StatusStatisticsDto>> GetStatusStatistics(Guid projectId);

    }
}
