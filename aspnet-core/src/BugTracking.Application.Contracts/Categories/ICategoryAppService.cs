using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BugTracking.Categories
{
    public interface ICategoryAppService : IApplicationService
    {
        Task<ListResultDto<CategoryDto>> GetListAsync();
        Task<List<CategoryDto>> GetListOnlyAsync();
        Task<List<CategoryDto>> GetListCategoryWithIssueAsync(Guid IdProject);
    }
}
