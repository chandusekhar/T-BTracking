using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BugTracking.Comments
{
    public interface ICommentService : IApplicationService
    {
        Task<CommentDto> GetByIdAsync(Guid id);
    }
}
