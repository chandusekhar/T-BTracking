using BugTracking.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Azures
{
    public interface IAzureRepository : IRepository<Azure, Guid>
    {

    }
}
