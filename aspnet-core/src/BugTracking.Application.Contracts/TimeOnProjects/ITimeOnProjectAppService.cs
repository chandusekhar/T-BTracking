using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BugTracking.TimeOnProjects
{
    public interface ITimeOnProjectAppService : IApplicationService
    {
        Task<TimeOnProjectDto> CreateAsync(CreateTimeOnProjectDto createTimeOnProjectDto);
    }
}
