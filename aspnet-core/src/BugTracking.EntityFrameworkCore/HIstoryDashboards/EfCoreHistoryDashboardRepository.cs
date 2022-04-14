using BugTracking.EntityFrameworkCore;
using BugTracking.HistoryDashboards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace BugTracking.HIstoryDashboards
{
     public class EfCoreHistoryDashboardRepository : EfCoreRepository<BugTrackingDbContext, HistoryDashboard>, IHistoryDashboardRepository
    {
        public EfCoreHistoryDashboardRepository(
        IDbContextProvider<BugTrackingDbContext> dbContextProvider)
        : base(dbContextProvider)
        {
        }
    }
}
