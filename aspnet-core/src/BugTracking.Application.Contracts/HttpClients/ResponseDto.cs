using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.HttpClients
{
    public class ResponseDto_Result
    {
        public bool Success { get; set; }
        public object Data { get; set; }
    }

    public class ResponseDto_SignIn
    {
        public string accessToken { get; set; }
        public string expiresIn { get; set; }
        public string tokenType { get; set; }
        public string refreshToken { get; set; }
        public string scope { get; set; }
        public DataResponse data { get; set; }
    }
    public class DataResponse
    {
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public string Name { get; set; }
    }

    public class ResponseDto_SmsOtp
    {
        public string SmsOtpToken { get; set; }
    }

    public class ResponseDto_OtpSms
    {
        public string TokenSmsOtp { get; set; }
    }

    public class ResponseDto_SignUp
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class ResponseDto_FromToken
    {
        public string Email { get; set; }
        public string Given_name { get; set; }
    }
};
