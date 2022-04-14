using BugTracking.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace BugTracking.TimeOnProjects
{
    public class EfCoreTimeOnProjectRepository : EfCoreRepository<BugTrackingDbContext, TimeOnProject, Guid>, ITimeOnProjectRepository
    {
        public EfCoreTimeOnProjectRepository(
           IDbContextProvider<BugTrackingDbContext> dbContextProvider)
           : base(dbContextProvider)
        {
        }
    }
}
