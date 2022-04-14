using System;
using Volo.Abp.Domain.Repositories;

namespace Volo.Abp.AuditLogging
{
    public interface IEnityPropertyRepository : IRepository<EntityPropertyChange, Guid>
    {
    }
}