using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BugTracking.Members
{
    public interface IMemberService : IApplicationService
    {
        Task<MemberDto> GetByIdAsync(Guid id);
        Task InsertByListAsync(CreateMemberByListDto Entity);
    }
}
