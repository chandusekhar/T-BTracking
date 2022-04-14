using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.Issues
{
    public interface IIssueRepository: IRepository<Issue, Guid>
    {
        Task<Issue> FindByIssueNameAsync(string name, Guid idProject);

        Task<List<Issue>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string filter = null
        );
        Task<List<Issue>> SearchIssueAsync(string text);
    }
}
