using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BugTracking.DetailAttachments
{
    public interface IDetailAttachmentRepository : IRepository<DetailAttachment, Guid>
    {
        Task<List<DetailAttachment>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string filter = null
        );
    }
}
