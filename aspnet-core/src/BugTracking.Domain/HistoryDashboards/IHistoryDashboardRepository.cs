using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.HistoryDashboards
{
    public interface IHistoryDashboardRepository : IRepository<HistoryDashboard>
    {
    }
}
