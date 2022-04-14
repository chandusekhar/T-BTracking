using BugTracking.EntityFrameworkCore;
using BugTracking.Projects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace BugTracking.Issues
{
    public class EfCoreIssueRepository : EfCoreRepository<BugTrackingDbContext, Issue, Guid>, IIssueRepository
    {
        public EfCoreIssueRepository(
            IDbContextProvider<BugTrackingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<Issue> FindByIssueNameAsync(string name,Guid idProject)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(issue => issue.Name == name&&issue.ProjectID==idProject);
        }

        public async Task<List<Issue>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .WhereIf(
                    !filter.IsNullOrWhiteSpace(),
                    issue => issue.Name.Contains(filter)
                 )
                .Skip(skipCount)
                .Take(maxResultCount)
                .OrderBy(sorting)
                .ToListAsync();
        }

        [Obsolete]
        public async Task<List<Issue>> SearchIssueAsync(string text)
        {
            string textEdit = text;
            var content = await DbContext.Issues.Where(x => x.Name.ToLower().Contains(textEdit.Replace(" ", "").ToLower()) || x.Description.Contains(textEdit.Replace(" ", ""))).ToListAsync();
            if (!content.Any() || string.IsNullOrWhiteSpace(textEdit))
            {
                return null;
            }
            else
            {
                return content;
            }
        }
    }
}
