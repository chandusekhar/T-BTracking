using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace BugTracking.TimeOnProjects
{
    public class TimeOnProjectManager : DomainService
    {
        private readonly ITimeOnProjectRepository _timeOnProjectsRepository;
        public TimeOnProjectManager(ITimeOnProjectRepository timeOnProjectsRepository)
        {
            _timeOnProjectsRepository = timeOnProjectsRepository;
        }
        public TimeOnProject CreateAsync(
          string applicationName,
          string urlPath,
          bool isActive,
          DateTime startTime,
          DateTime endTime,
          string uniqueName
          )
        {
            return new TimeOnProject(
                GuidGenerator.Create(),
                applicationName,
                urlPath,
                isActive,
                startTime,
                endTime,
                uniqueName
            );
        }
    }
}
