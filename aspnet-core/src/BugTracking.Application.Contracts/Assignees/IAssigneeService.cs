using BugTracking.IShareDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BugTracking.Assignees
{
    public interface IAssigneeService : IApplicationService
    {
        Task<AssigneeDto> GetByIdAsync(Guid id);
        Task CreateByListAsync(string[] assginList, Guid idIssue, bool IsDefault);
        Task<AssigneeDto> CreateAsync(CreateDto input);

        Task UpdateAsync(Guid id, UpdateDto input);

        Task DeleteAsync(Guid id, Guid idIssue);
    }
}
