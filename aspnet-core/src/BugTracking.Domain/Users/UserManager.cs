using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;

namespace BugTracking.Users
{
    public class UserManager : DomainService
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityUserRepository _identityUserReposity;
        private readonly IdentityUserManager _identityUserManager;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public static IServiceProvider ServiceProvider { get; private set; }
        public UserManager(
            IHttpContextAccessor httpContextAccessor,
            ITMTCurrentUser tmtCurrentUser,
        IIdentityUserRepository identityUserReposity,
            IdentityUserManager identityUserManager,
        IUserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _tmtCurrentUser = tmtCurrentUser;
            _identityUserReposity = identityUserReposity;
            _userRepository = userRepository;
            _identityUserManager = identityUserManager;
        }

        public async Task<AppUser> FindByNameAsync(string name)
        {
            var nameUser = await _userRepository.FindByNameAsync(name);
            return nameUser;
        }

        public async Task<System.Collections.Generic.List<string>> FindRoleByUserId(string id)
        {
           var roleName =  await _identityUserReposity.GetRoleNamesAsync(id);
            return roleName;
        }
       
        public async Task<bool> CheckRoleByUserId(string userID,string roleName)
        {
            var getUser = await _identityUserManager.FindByIdAsync(userID);
            var getRoleUser = await _identityUserManager.GetRolesAsync(getUser);
            return getRoleUser.Contains(roleName);
        }

        public void isCreator(string entityId, dynamic repository)
        {
            var x = repository;
        }

        public void isAllowed(string action, string entityId, dynamic repository)
        {


            switch(action)
            {
                case "delete":
                    if (CheckRoleByUserId(_tmtCurrentUser.Id, "user").Result)
                    {
                        isCreator(entityId, repository);
                        throw new UserFriendlyException("You are not authorized to delete this content!");
                    }
                    break;

                case "create":
                    Console.WriteLine("It is 2");
                    break;
                case "update":
                    Console.WriteLine("It is 2");
                    break;
            }
        }
    }
}