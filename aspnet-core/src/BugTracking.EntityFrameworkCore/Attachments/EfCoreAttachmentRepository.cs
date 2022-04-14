using BugTracking.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace BugTracking.Attachments
{
    public class EfCoreAttachmentRepository : EfCoreRepository<BugTrackingDbContext, Attachment, Guid>,IAttachmentRepository
    {
        public EfCoreAttachmentRepository(
            IDbContextProvider<BugTrackingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Attachment>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            var dbSet = await GetDbSetAsync();//
            return await dbSet
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }
    }
}
