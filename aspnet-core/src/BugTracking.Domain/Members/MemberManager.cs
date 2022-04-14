using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace BugTracking.Members
{
    public class MemberManager : DomainService
    {
        private readonly IMemberRepository _memberRepository;
        public MemberManager(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }
        public async Task<Member> CreateAsync(
            Guid projectID,
            string userID
            )
        {
            var query = await _memberRepository.FindAsync(x => x.ProjectID == projectID && x.UserID == userID);
            return new Member(
                GuidGenerator.Create(),
                projectID,
                userID
            );
        }

    }
}
