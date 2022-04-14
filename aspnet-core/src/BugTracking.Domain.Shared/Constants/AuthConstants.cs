using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Constants
{
    public static class AuthConstants
    {
        public static class PhoneNumberTokenSms
        {
            public const string PhoneNumber = "phone_number";
            //   public const string SmsOtpToken = "sms_otp_token";
            public const string OtpCode = "otp_code";
            public const string GrantType = "phone_number_token_sms";
        }
        public static class PhoneNumberPassword
        {
            public const string PhoneNumber = "phone_number";
            public const string Password = "password";
            public const string GrantType = "phone_number_password";

        }
        public static class Facebook
        {
            public const string FacebookToken = "facebook_token";
            public const string GrantType = "facebook";
        }
        public static class Google
        {
            public const string GoogleToken = "google_token";
            public const string GrantType = "google";
        }
    }
}
