using BugTracking.Azure;
using BugTracking.UserInforTFS;
using BugTracking.Users;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMT.Security.Users;

namespace BugTracking.UserInforTfs
{
    [Authorize("BugTracking.Users")]
    public class UserInforTfsAppService : BugTrackingAppService, IUserInforTfsAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly IUserInforTfsRepository _userInforTfsRepository;
        private readonly UserInforTfsManager _userInforTfsManager;
        private readonly AzureAppService _azureAppService;
        public UserInforTfsAppService(
            IUserRepository userRepository, 
            ITMTCurrentUser tmtCurrentUser, 
            IUserInforTfsRepository userInforTfsRepository,
            UserInforTfsManager userInforTfsManager,
            AzureAppService azureAppService)
        {
            _userRepository = userRepository;
            _tmtCurrentUser = tmtCurrentUser;
            _userInforTfsRepository = userInforTfsRepository;
            _userInforTfsManager = userInforTfsManager;
            _azureAppService = azureAppService;
        }
        public UserInforTFS.UserInforTfs GetAsync(string UserId)
        {
            var userInfor = _userInforTfsRepository.FirstOrDefault(x => x.UserId == UserId);
            return userInfor;
        }
        public async Task<UserInforTfsDto> CreateAsync(CreateUserInforTfsDto createUserInforTfsDto)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            UserInforTFS.UserInforTfs userInfor;
            if (_userInforTfsRepository.Any(x => x.UserId == CurrentUserId))
            {
                if(_userInforTfsRepository.FirstOrDefault(x=>x.UserId == CurrentUserId).PAT != createUserInforTfsDto.PAT)
                {
                    await _azureAppService.GetProjectByPAT(createUserInforTfsDto.PAT);
                }
                userInfor = _userInforTfsRepository.FirstOrDefault(x => x.UserId == CurrentUserId);
                userInfor.UniqueName = createUserInforTfsDto.UniqueName.Replace(" ", "");
                userInfor.PAT = createUserInforTfsDto.PAT.Replace(" ", "");

                await _userInforTfsRepository.UpdateAsync(userInfor);

            }
            else
            {
                await _azureAppService.GetProjectByPAT(createUserInforTfsDto.PAT);
                userInfor = await _userInforTfsManager.CreateAsync(
                CurrentUserId,
                createUserInforTfsDto.UniqueName,
                createUserInforTfsDto.PAT
            );

                await _userInforTfsRepository.InsertAsync(userInfor);

            }
            return ObjectMapper.Map<UserInforTFS.UserInforTfs, UserInforTfsDto>(userInfor);
        }

        public async Task<UserInforTfsDto> UpdateAsync(CreateUserInforTfsDto createUserInforTfsDto)
        {
            var CurrentUserId = _tmtCurrentUser.GetId();
            await _azureAppService.GetTMTProjects(CurrentUserId);
            var userInfor = _userInforTfsRepository.FirstOrDefault(x => x.UserId == CurrentUserId);
            userInfor.UniqueName = createUserInforTfsDto.UniqueName;
            userInfor.PAT = createUserInforTfsDto.PAT;

            await _userInforTfsRepository.UpdateAsync(userInfor);

            return ObjectMapper.Map<UserInforTFS.UserInforTfs, UserInforTfsDto>(userInfor);
        }
        public async Task DeleteAsync(Guid Id)
        {
            await _userInforTfsRepository.DeleteAsync(Id);
        }
    }
}
