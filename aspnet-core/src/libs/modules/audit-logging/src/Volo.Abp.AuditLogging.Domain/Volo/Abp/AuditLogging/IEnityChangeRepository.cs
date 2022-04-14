using System;
using Volo.Abp.Domain.Repositories;

namespace Volo.Abp.AuditLogging
{
    public interface IEnityChangeRepository : IRepository<EntityChange, Guid>
    {
    }
}