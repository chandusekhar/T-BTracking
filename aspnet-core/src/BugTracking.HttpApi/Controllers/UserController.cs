using BugTracking.HttpClients;
using BugTracking.IShareDto;
using BugTracking.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Http;

namespace BugTracking.Controllers
{
    [IgnoreAntiforgeryToken]
    [Route("api/user")]
    public class UserController : AbpController
    {
        private readonly IUserAppService _userService;

        public UserController(IUserAppService userService)
        {
            _userService = userService;
        }

        [HttpPost("sign-in")]
        [ProducesResponseType(typeof(ResponseDto_SignIn), 200)]
        [ProducesResponseType(typeof(RemoteServiceErrorResponse), 400)]
        public async Task<ActionResult> Login([FromBody] SignInUserDto input)
        {
            var result = await _userService.SignIn(input);
            if (result.Success)
            {
                //var x = _httpContextAccessor.HttpContext.User.FindFirst("sub").Value;
                //Console.WriteLine(x);
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Data);
            }
        }

        [HttpPost("sign-up/send-otp-sms")]
        public async Task<ActionResult> SendOtpSms_SignUp([FromBody] SendOtpSmsDto input)
        {
            var result = await _userService.SendOtpSms(input);
            if (result.Success)
            {
                //Console.WriteLine(_httpContextAccessor.HttpContext.User.FindFirst("sub").Value);
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Data);
            }
        }

        [HttpPost("sign-up/verify-otp-sms")]
        [ProducesResponseType(typeof(ResponseDto_SmsOtp), 200)]
        [ProducesResponseType(typeof(RemoteServiceErrorResponse), 400)]
        public async Task<ActionResult> VerifyOtpSms([FromBody] SignUpUserDto_VerifyOtpSms input)
        {
            var result = await _userService.VerifyOtpSms(input);
            if (result.Success)
            {
                //Console.WriteLine(_httpContextAccessor.HttpContext.User.FindFirst("sub").Value);
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Data);
            }
        }

        [HttpPost("sign-up")]
        [ProducesResponseType(typeof(ResponseDto_SignUp), 200)]
        [ProducesResponseType(typeof(RemoteServiceErrorResponse), 400)]
        public async Task<ActionResult> SignUp([FromBody] SignUpUserDto input)
        {
            var result = await _userService.SignUp(input);
            if (result.Success)
            {
                //Console.WriteLine(_httpContextAccessor.HttpContext.User.FindFirst("sub").Value);
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Data);
            }
        }

        [HttpPost("user/reset-password/send-otpsms")]
        public async Task<ActionResult> SendOtpSms_ForgetPassword([FromBody] SendOtpSmsDto_Password input)
        {
            var result = await _userService.SendOtpSms_ResetPassword(input);
            if (result.Success)
            {
                //Console.WriteLine(_httpContextAccessor.HttpContext.User.FindFirst("sub").Value);
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Data);
            }
        }

        [HttpPost("user/reset-password/verify-otpsms")]
        public async Task<ActionResult> VerifyOtpSms_ResetPassword([FromBody] VerifyOtpSms_Password input)
        {
            var result = await _userService.VerifyOtpSms_ResetPassword(input);
            if (result.Success)
            {
                //Console.WriteLine(_httpContextAccessor.HttpContext.User.FindFirst("sub").Value);
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Data);
            }
        }

        [HttpPost("user/reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto input)
        {
            var result = await _userService.ResetPassword(input);
            if (result.Success)
            {
                //Console.WriteLine(_httpContextAccessor.HttpContext.User.FindFirst("sub").Value);
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Data);
            }
        }

        [HttpPost("user/change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePassword input)
        {
            var result = await _userService.ChangePassword(input);
            if (result.Success)
            {
                //Console.WriteLine(_httpContextAccessor.HttpContext.User.FindFirst("sub").Value);
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Data);
            }
        }


        //[Authorize]
        //[HttpGet("profile")]
        //public async Task<ActionResult> GetProfile()
        //{
        //    var result = await _userService.GetProfile();
        //    if (result.Success)
        //    {
        //        //Console.WriteLine(_httpContextAccessor.HttpContext.User.FindFirst("sub").Value);
        //        return Ok(result.Data);
        //    }
        //    else
        //    {
        //        return BadRequest(result.Data);
        //    }
        //}

        //[HttpGet("get-list-user")]
        //public async Task<ActionResult> GetListAsync()
        //{
        //    var result = await _userService.GetListAsync();
        //    return Ok(result);
        //}
        [HttpGet("get-list-user")]
        public Task<List<UserDto>> GetListAsync()
        {
            return _userService.GetListAsync();
        }
        [HttpGet("get-profile")]
        public async Task<UserDto> GetAsync()
        {
            var result = await _userService.GetAsync();
            return result;
        }
        [HttpGet("get-profile-by-Id")]
        public async Task<UserDto> GetAsyncById(string id)
        {
            var result = await _userService.GetAsyncById(id);
            return result;
        }
        [HttpGet("get-user-follow-by-issueId")]
        public  ListResultDto<UserDto> GetUserFollowAsync(Guid IssueId)
        {
            var result = _userService.GetListUserFollowIssue(IssueId);
            return result;
        }
        [HttpPut("update-profile")]
        public async Task<UserDto> UpdateAsync(UpdateUserDto updateDto)
        {
            return await _userService.UpdateAsync(updateDto);
        }

        [HttpGet("get-list-user-by-project-id")]
        public async Task<ListResultDto<UserDto>> GetListAsyncByProjectId(Guid projectId)
        {
            return await _userService.GetListAsyncByProjectId(projectId);
        }

        [HttpGet("get-list-user-add-project")]
        public async Task<ListResultDto<UserDto>> GetListUserAddProject(Guid ProjectId)
        {
              return await _userService.GetListUserAddProject(ProjectId);
            
        }

        [HttpGet("get-list-user-add-assign-issue")]
        public ActionResult GetListUserNotInAssign(Guid projectId, Guid issueID)
        {
            var result = _userService.GetListUserNotInAssign(projectId, issueID);
            return Ok(result);
        }

        //public PagedResultDto<UserDto> GetListUserByIdProjectSearch(GetListDto input, Guid IdProject)
        //{
        //    return _userService.GetListUserByIdProjectSearch(input, IdProject);

        //}
        [HttpGet("get-list-user-by-id-project-search")]
        public async Task<List<UserDto>> GetListUserByIdProjectSearch(GetListDto input, Guid IdProject)
        {
            var result = await _userService.GetListUserByIdProjectSearch(input, IdProject);
            return result;
        }

        [HttpGet("get-list-createrIssue-by-project-id")]
        public async Task<ListResultDto<UserDto>> GetListCreaterByProjectId(Guid projectId)
        {
            return await _userService.GetListCreaterByProjectId(projectId);
        }
        [HttpGet("check-admin")]
        public async Task<bool> GetCheckAdmin()
        {
            return await _userService.GetCheckAdmin();
        }
     
    }
}