using BugTracking.IShareDto;
using BugTracking.Issues;
using Microsoft.TeamFoundation.Build.WebApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BugTracking.Departments
{
    public interface IDepartmentAppService : IApplicationService
    {
        Task<DepartmentDto> CreateAsync(CreateDepartmentDto input);
        Task UpdateAsync(Guid id, UpdateDepartmentDto input);
        Task<ListResultDto<DepartmentDto>> GetListAsync();
        Task<List<IssuesDto>> GetListIssueById(Guid IdDepartment);
        Task<List<DepartmentDto>> GetListDepartment(string filter);
        Task DeleteAsync(Guid id);
        List<string> getNameDepartmentByManager(string IdUser);
        bool GetCheckManager(string UserId);
        Task<PagedResultDto<IssuesDto>> GetListIssueAll(GetListDto input, string IdProject, string IdStatus, string IdCate, string IdUser, string IdDepartment, string IdTeam, bool IsAss);
    }
}
