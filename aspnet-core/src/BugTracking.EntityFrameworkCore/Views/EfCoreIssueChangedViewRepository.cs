using BugTracking.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace BugTracking.Views
{
    public class EfCoreIssueChangedViewRepository : EfCoreRepository<BugTrackingDbContext, IssueChangedView>, IIssueChangedViewRepository
    {
        public EfCoreIssueChangedViewRepository(
           IDbContextProvider<BugTrackingDbContext> dbContextProvider)
           : base(dbContextProvider)
        {
        }
    }
}
