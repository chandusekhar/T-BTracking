using BugTracking.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace BugTracking.Azures
{
    public class EfCoreAzureRepository : EfCoreRepository<BugTrackingDbContext, Azure, Guid>, IAzureRepository
    {
        public EfCoreAzureRepository(
            IDbContextProvider<BugTrackingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
