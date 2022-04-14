using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Volo.Abp.AuditLogging.EntityFrameworkCore
{
    public class EfCoreEnityChangeRepository : EfCoreRepository<IAuditLoggingDbContext, EntityChange, Guid>, IEnityChangeRepository
    {
        public EfCoreEnityChangeRepository(IDbContextProvider<IAuditLoggingDbContext> dbContextProvider)
         : base(dbContextProvider)
        {
        }
    }
}