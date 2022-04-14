using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace BugTracking.UserInforTFS
{
    public class UserInforTfsManager : DomainService
    {
        private readonly IUserInforTfsRepository _userInforTfsRepository;
        public UserInforTfsManager(IUserInforTfsRepository userInforTfsRepository)
        {
            _userInforTfsRepository = userInforTfsRepository;
        }
        public async Task<UserInforTfs> CreateAsync(
         [NotNull] string userId,
          string uniqueName,
          string pAT
          )
        {

            Check.NotNullOrWhiteSpace(userId, nameof(userId));
            Check.NotNullOrWhiteSpace(uniqueName, nameof(uniqueName));
            Check.NotNullOrWhiteSpace(pAT, nameof(pAT));

            var exist = await _userInforTfsRepository.FindByNameAsync(userId,uniqueName);
            if (exist != null)
            {
                throw new Exception("User Infor already exist!");
            }

            return new UserInforTfs(
                GuidGenerator.Create(),
                userId,
                uniqueName,
                pAT
            );
        }
    }
}
