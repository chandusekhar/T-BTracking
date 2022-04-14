using BugTracking.HttpClients;
using BugTracking.IShareDto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BugTracking.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task<ResponseDto_Result> SignIn(SignInUserDto input);

        Task<ResponseDto_Result> SendOtpSms(SendOtpSmsDto input);

        Task<ResponseDto_Result> VerifyOtpSms(SignUpUserDto_VerifyOtpSms input);

        Task<ResponseDto_Result> SignUp(SignUpUserDto input);

        Task<ResponseDto_Result> SendOtpSms_ResetPassword(SendOtpSmsDto_Password input);

        Task<ResponseDto_Result> VerifyOtpSms_ResetPassword(VerifyOtpSms_Password input);

        Task<ResponseDto_Result> ResetPassword(ResetPasswordDto input);

        Task<ResponseDto_Result> ChangePassword(ChangePassword input);

        ListResultDto<UserDto> GetListUserNotInAssign(Guid projectId, Guid issueID);

        Task<UserDto> GetAsync();
        Task<UserDto> GetAsyncById(string id);
        Task<bool> GetCheckAdmin();

        Task<UserDto> UpdateAsync(UpdateUserDto updateDto);

        Task<List<UserDto>> GetListAsync();

        Task<ListResultDto<UserDto>> GetListUserAddProject(Guid ProjectId);
        //Task<ActionResult> GetListUserAddProject(Guid ProjectId);
        //Task<string> getUserName(string userId);
        Task<ListResultDto<UserDto>> GetListAsyncByProjectId(Guid projectId);
        Task<ListResultDto<UserDto>> GetListCreaterByProjectId(Guid projectId);

        Task<List<UserDto>> GetListUserByIdProjectSearch(GetListDto input, Guid IdProject);

        // PagedResultDto<UserDto> GetListUserByIdProjectSearch(GetListDto input, Guid IdProject);
        //Task<ResponseDto_Result> GetProfile();
        ListResultDto<UserDto> GetListUserFollowIssue(Guid IssueId);
    }
}