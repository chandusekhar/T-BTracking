using BugTracking.Assignees;
using BugTracking.Azure;
using BugTracking.Azures;
using BugTracking.Follows;
using BugTracking.HttpClients;
using BugTracking.IShareDto;
using BugTracking.Issues;
using BugTracking.Members;
using BugTracking.Projects;
using BugTracking.Statuss;
using BugTracking.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TMT.Security.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace BugTracking.Services
{
    [RemoteService(IsEnabled = false)]
    [Authorize("BugTracking.Users")]
    [AllowAnonymous]
    public class UserAppService : BugTrackingAppService, IUserAppService
    {
        private readonly IdentityUserManager _identityUserManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpClientService _httpClient;
        private readonly IConfiguration _configuration;
        private readonly UserManager _userManager;
        private readonly IIssueRepository _issueRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IAssigneeRepository _assineeRepository;
        private readonly ITMTCurrentUser _tmtCurrentUser;
        private readonly IPermissionManager _permissionManager;
        private readonly IFollowRepository _followRepository;
        public UserAppService(
            IPermissionManager permissionManager,
            IdentityUserManager identityUserManager,
            IMemberRepository memberRepository,
            IHttpClientFactory httpClientFactory,
            IHttpClientService httpClient,
            IIssueRepository issueRepository,
             IStatusRepository statusRepository,
            IConfiguration configuration,
            UserManager userManager,
            IUserRepository userRepository,
            IAssigneeRepository assineeRepository,
            ITMTCurrentUser tmtCurrentUser,
            IFollowRepository followRepository)
        {
            _permissionManager = permissionManager;
            _identityUserManager = identityUserManager; ;
            _httpClientFactory = httpClientFactory;
            _userRepository = userRepository;
            _httpClient = httpClient;
            _configuration = configuration;
            _userManager = userManager;
            _memberRepository = memberRepository;
            _assineeRepository = assineeRepository;
            _tmtCurrentUser = tmtCurrentUser;
            _issueRepository = issueRepository;
            _statusRepository = statusRepository;
            _followRepository = followRepository;
        }
        public async Task<UserDto> GetAsync()
        {
            string id = _tmtCurrentUser.GetId();
            var user = await _userRepository.GetAsync(id);

            return ObjectMapper.Map<AppUser, UserDto>(user);
        }
        public async Task<UserDto> GetAsyncById(string id)
        {
            var user = await _userRepository.GetAsync(id);

            return ObjectMapper.Map<AppUser, UserDto>(user);
        }

        public async Task<List<UserDto>> GetListAsync()
        {
            IQueryable<AppUser> usertQueryable = await _userRepository.GetQueryableAsync();
            var query = from user in usertQueryable
                        select new { user};
            var queryResult = await AsyncExecuter.ToListAsync(query);
            var userDtos = queryResult.Select(result =>
            {
                var UserDto = ObjectMapper.Map<AppUser, UserDto>(result.user);
                return UserDto;
            }).ToList();
            return userDtos;

        }
        public async Task<ListResultDto<UserDto>> GetListUserAddProject(Guid ProjectId)
        {
            var users = await _userRepository.GetListAsync();
            var members = _memberRepository.Where(member => member.ProjectID == ProjectId).ToList();

            var query = from user in users
                        where members.FirstOrDefault(member => member.UserID == user.Id) == null 
                        select user;

            return new ListResultDto<UserDto>(ObjectMapper.Map<List<AppUser>, List<UserDto>>(query.ToList()));
        }
        public ListResultDto<UserDto> GetListUserFollowIssue(Guid IssueId)
        {
            var query = from user in _userRepository
                        join follow in _followRepository on user.Id equals follow.UserID
                        where follow.IssueID == IssueId
                        select user;

            return new ListResultDto<UserDto>(ObjectMapper.Map<List<AppUser>, List<UserDto>>(query.ToList()));
        }
        public async Task<ListResultDto<UserDto>> GetListAsyncByProjectId(Guid projectId)
        {
            IQueryable<AppUser> userQueryable = await _userRepository.GetQueryableAsync();
            IQueryable<Member> memberQueryable = await _memberRepository.GetQueryableAsync();
            //Create a query
            var query = from user in userQueryable
                        join member in memberQueryable on user.Id equals member.UserID
                        where member.ProjectID == projectId
                        select user;

            //Execute the query to get list of people
            var users = query.ToList();

            //Convert to DTO and return to the client
            return new ListResultDto<UserDto>(ObjectMapper.Map<List<AppUser>, List<UserDto>>(users));
        }
        public ListResultDto<UserDto> GetListUserNotInAssign(Guid projectId, Guid issueID)
        {
            var users = new List<AppUser> { };
            foreach (AppUser user in _userRepository)
            {
                foreach (Member member in _memberRepository.Where(member => member.ProjectID == projectId))
                {
                    if (member.UserID == user.Id)
                    {
                        users.Add(user);
                        foreach (Assignee assignee in _assineeRepository.Where(ass => ass.IssueID == issueID))
                        {
                            if (user.Id == assignee.UserID)
                            {
                                users.Remove(user);
                            }
                        }
                    }
                }

            }
            return new ListResultDto<UserDto>(ObjectMapper.Map<List<AppUser>, List<UserDto>>(users));
        }
        public async Task<bool> GetCheckAdmin()
        {
            var CurrentUserID = _tmtCurrentUser.GetId();
            return await _userManager.CheckRoleByUserId(CurrentUserID, "admin");
        }
        public async Task<string> getUserName(string userId)
        {
            var user = await _userRepository.GetAsync(userId);
            string nameUser = user.Name;
            return nameUser;
        }
        public async Task<List<UserDto>> GetListUserByIdProjectSearch(GetListDto input, Guid IdProject)
        {
            if (input.Filter.IsNullOrWhiteSpace() || input.Filter == "null")
            {
                input.Filter = "";
            }
            IQueryable<AppUser> userQueryable = await _userRepository.GetQueryableAsync();
            IQueryable<Member> memberQueryable = await _memberRepository.GetQueryableAsync();
            var query = from user in userQueryable
                        join member in memberQueryable on user.Id equals member.UserID into termMember
                        from u in termMember.DefaultIfEmpty()
                        where u.ProjectID == IdProject && (user.Name.Contains(input.Filter) || user.Email.Contains(input.Filter) || user.PhoneNumber.Contains(input.Filter))
                        orderby user.CreationTime descending
                        select new
                        {
                            user
                        };

            var queryResult = await AsyncExecuter.ToListAsync(query);
            var UserDtos = queryResult.Select(result =>
            {
                var UserDto = ObjectMapper.Map<AppUser, UserDto>(result.user);
                var member = _memberRepository.Where(u => u.UserID == UserDto.Id).FirstOrDefault();
                var status = _statusRepository.Where(u => u.Name == "Closed").FirstOrDefault();
                //  decimal countIssueAssign = _assineeRepository.Where(x => x.UserID == result.user.Id).Count();
                decimal countIssueAssign = 0;
                decimal IssueClosedAssign = 0;
                UserDto.Id = member.UserID;
                UserDto.Name = result.user.Name;
                UserDto.Email = result.user.Email;
                UserDto.PhoneNumber = result.user.PhoneNumber;
                UserDto.CreationTime = member.CreationTime;
                UserDto.IDMember = member.Id;
                UserDto.CreatedBy = member.CreatorId != null ? _userRepository.FirstOrDefault(x => x.Id == member.CreatorId).Name : "";

                if (_userManager.CheckRoleByUserId(result.user.Id, "admin").Result) UserDto.IdUserAdmin = result.user.Id;
                decimal countIssueCreate = _issueRepository.Where(issue => issue.CreatorId == result.user.Id && issue.ProjectID == IdProject).Count();
                foreach (Issue issue in _issueRepository.Where(issue => issue.ProjectID == IdProject))
                {
                    foreach (Assignee assignee in _assineeRepository.Where(x => x.UserID == result.user.Id && x.IssueID == issue.Id))
                    {
                        if (assignee.UserID != issue.CreatorId)
                        {
                            countIssueAssign++;
                        }
                    }
                }

                var listIssueAssign = _assineeRepository.Where(x => x.UserID == result.user.Id).ToList();
                foreach (Assignee assignee in listIssueAssign)
                {
                    var a = _issueRepository.Where(x => x.Id == assignee.IssueID && (x.FinishDate) != null && x.ProjectID == IdProject).ToList();
                    for (int i = 0; i < a.Count(); i++)
                    {
                        if (a[i].CreatorId != assignee.UserID)
                            IssueClosedAssign++;
                    }
                }

                decimal IssueClosedCreate = _issueRepository.Where(x => x.FinishDate != null && x.CreatorId == result.user.Id && x.ProjectID == IdProject).Count();
                decimal issueFinish;
                if ((countIssueAssign + countIssueCreate) == 0)
                {
                    issueFinish = 0;
                }
                else
                {
                    issueFinish = ((IssueClosedCreate + IssueClosedAssign) / (countIssueAssign + countIssueCreate)) * 100;
                }

                UserDto.IssueFinish = (long)issueFinish;
                UserDto.CountIssue = (int)(countIssueAssign + countIssueCreate);
                return UserDto;
            }).ToList();
            return UserDtos;
        }

        #region Login
        [AllowAnonymous]
        public async Task<ResponseDto_Result> SignIn(SignInUserDto input)
        {
            var bodyJson = JsonConvert.SerializeObject(input);
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(bodyJson);
            dic.Add("clientId", _configuration.GetSection("AuthServer")["SwaggerClientId"]);
            dic.Add("clientSecret", _configuration.GetSection("AuthServer")["SwaggerClientSecret"]);
            dic.Add("scope", "");
            var resUser = await _httpClient.ResponseWithModel<ResponseDto_SignIn>(dic, "/api/v1/sign-in/password", _configuration.GetSection("AuthServer")["Authority"]);
            if (resUser.Success)
            {
                var json = JsonConvert.SerializeObject(resUser.Data);
                var resParse = JsonConvert.DeserializeObject<ResponseDto_SignIn>(json);
                try
                {
                    var user = await _userRepository.GetAsync(resParse.data.Id);
                    resParse.data.Email = user.Email;
                    resParse.data.Name = user.Name;
                    resParse.data.IsAdmin = await _userManager.CheckRoleByUserId(resParse.data.Id, "admin");
                }
                catch
                {
                    //var profile = await _azureAppService.getProfile();
                    var client = _httpClientFactory.CreateClient();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + resParse.accessToken);
                    client.BaseAddress = new Uri(_configuration.GetSection("AuthServer")["Authority"]);
                    var response = await client.GetAsync(_configuration.GetSection("AuthServer")["Authority"] + "/api/v1/user-profile");
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var dataParse = JsonConvert.DeserializeObject<ResponseDto_FromToken>(jsonResponse);
                    IdentityUser identityUser1 = new IdentityUser(resParse.data.Id, dataParse.Email, dataParse.Email, dataParse.Given_name, resParse.data.PhoneNumber);
                    await _identityUserManager.CreateAsync(identityUser1);
                    await _permissionManager.SetForUserAsync(resParse.data.Id, "BugTracking.Users", true);
                    await _identityUserManager.AddToRoleAsync(identityUser1, "User");
                    resParse.data.Email = dataParse.Email;
                    resParse.data.Name = dataParse.Given_name;
                }
                return new ResponseDto_Result
                {
                    Success = true,
                    Data = resParse
                };
            }
            else
            {
                return resUser;
            }

        }
        #endregion

        #region SignUp
        [AllowAnonymous]
        public async Task<ResponseDto_Result> SendOtpSms(SendOtpSmsDto input)
        {
            //NONE RES
            return await _httpClient.ResponseWithModel<ResponseDto_SignUp>(input, "/api/v1/sign-up/send-otpsms", _configuration.GetSection("AuthServer")["Authority"]);
        }
        [AllowAnonymous]
        public async Task<ResponseDto_Result> VerifyOtpSms(SignUpUserDto_VerifyOtpSms input)
        {
            var Host = _configuration.GetSection("AuthServer")["Authority"];
            var rs= await _httpClient.ResponseWithModel<ResponseDto_SmsOtp>(input, "/api/v1/sign-up/verify-otpsms", Host);
            return rs;
        }
        [AllowAnonymous]
        public async Task<ResponseDto_Result> SignUp(SignUpUserDto input)
        {
            var resData = await _httpClient.ResponseWithModel<ResponseDto_SignUp>(input, "/api/v1/sign-up", _configuration.GetSection("AuthServer")["Authority"]);
            if (resData.Success)
            {
                //var profile = await _azureAppService.getProfile();
                var json = JsonConvert.SerializeObject(resData.Data);
                var data = JsonConvert.DeserializeObject<ResponseDto_SignUp>(json);
                IdentityUser identityUser = new IdentityUser(data.Id, data.Email, data.Email, data.Name, data.PhoneNumber);
                await _identityUserManager.CreateAsync(identityUser);
                await _permissionManager.SetForUserAsync(data.Id, "BugTracking.Users", true);
                await _identityUserManager.AddToRoleAsync(identityUser, "User");
            }
            return resData;
        }
        #endregion

        #region Reset password
        [AllowAnonymous]
        public async Task<ResponseDto_Result> SendOtpSms_ResetPassword(SendOtpSmsDto_Password input)
        {
            //NONE RES
            return await _httpClient.ResponseWithModel<ResponseDto_SignUp>(input, "/api/v1/user-profile/reset-password/send-otpsms", _configuration.GetSection("AuthServer")["Authority"]);
        }
        [AllowAnonymous]
        public async Task<ResponseDto_Result> VerifyOtpSms_ResetPassword(VerifyOtpSms_Password input)
        {
            return await _httpClient.ResponseWithModel<ResponseDto_OtpSms>(input, "/api/v1/user-profile/reset-password/verify-otpsms", _configuration.GetSection("AuthServer")["Authority"]);
        }
        [AllowAnonymous]
        public async Task<ResponseDto_Result> ResetPassword(ResetPasswordDto input)
        {
            //NONE RES
            return await _httpClient.ResponseWithModel<ResetPasswordDto>(input, "/api/v1/user-profile/reset-password", _configuration.GetSection("AuthServer")["Authority"]);
        }
        #endregion

        public async Task<ResponseDto_Result> ChangePassword(ChangePassword input)
        {
            //NONE RES
            return await _httpClient.ResponseWithModel<ChangePassword>(input, "/api/v1/user-profile/change-password", _configuration.GetSection("AuthServer")["Authority"]);
        }

        public async Task<UserDto> UpdateAsync(UpdateUserDto updateDto)
        {
            string id = _tmtCurrentUser.GetId();
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                throw new UserFriendlyException("Invalid User!");
            }
            user.Name = updateDto.Name;
            user.Email = updateDto.Email;
            await _userRepository.UpdateAsync(user);
            return ObjectMapper.Map<AppUser, UserDto>(user);
        }

        public async Task<ListResultDto<UserDto>> GetListCreaterByProjectId(Guid projectId)
        {
            IQueryable<AppUser> userQueryable = await _userRepository.GetQueryableAsync();
            IQueryable<Issue> issueQueryable = await _issueRepository.GetQueryableAsync();
            //Create a query
            var query = from user in userQueryable
                        join issue in issueQueryable on user.Id equals issue.CreatorId
                        where issue.ProjectID == projectId
                        select user;

            //Execute the query to get list of people
            var users = query.Distinct().ToList();

            //Convert to DTO and return to the client
            return new ListResultDto<UserDto>(ObjectMapper.Map<List<AppUser>, List<UserDto>>(users));
        }
    }
}