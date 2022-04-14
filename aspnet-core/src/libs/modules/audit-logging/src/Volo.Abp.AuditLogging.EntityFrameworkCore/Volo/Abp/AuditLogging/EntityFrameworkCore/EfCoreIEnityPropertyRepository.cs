using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Volo.Abp.AuditLogging.EntityFrameworkCore
{
    public class EfCoreIEnityPropertyRepository : EfCoreRepository<IAuditLoggingDbContext, EntityPropertyChange, Guid>, IEnityPropertyRepository
    {
        public EfCoreIEnityPropertyRepository(IDbContextProvider<IAuditLoggingDbContext> dbContextProvider)
        : base(dbContextProvider)
        {
        }
    }
}