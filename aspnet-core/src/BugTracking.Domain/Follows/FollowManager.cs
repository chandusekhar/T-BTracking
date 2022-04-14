using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace BugTracking.Follows
{
    public class FollowManager : DomainService
    {
        private readonly IFollowRepository _followRepository;
        public FollowManager(IFollowRepository followRepository)
        {
            _followRepository = followRepository;
            //push change
        }
        public Follow CreateAsync(
            Guid issueID,
            string userID
            )
        {
            return new Follow(
                GuidGenerator.Create(),
                issueID,
                userID
            );
        }
    }
}
