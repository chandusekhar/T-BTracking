using BugTracking.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace BugTracking.HistoryViews
{
    public class EfCoreHistoryViewRepository : EfCoreRepository<BugTrackingDbContext, HistoryView>, IHistoryViewRepository
    {
        public EfCoreHistoryViewRepository(
                IDbContextProvider<BugTrackingDbContext> dbContextProvider)
                : base(dbContextProvider)
        {
        }
    }
}
