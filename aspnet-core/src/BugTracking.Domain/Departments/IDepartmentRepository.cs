using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Departments
{
    public interface IDepartmentRepository : IRepository<Department, Guid>
    {
        Task<List<Department>> GetListAsync(
         int skipCount,
         int maxResultCount,
         string sorting,
         string filter = null
     );
    }
}
