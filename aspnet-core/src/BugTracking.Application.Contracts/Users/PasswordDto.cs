namespace BugTracking.Users
{
    public class ChangePassword
    {
        public string CurrentPassword { get; set; }
        public string NewPassPassword { get; set; }
    }

    public class ResetPasswordDto
    {
        public string NewPassword { get; set; }
        public string PhoneNumber { get; set; }
        public string TokenSmsOtp { get; set; }
        public string OtpCode { get; set; }
    }

    public class SendOtpSmsDto_Password
    {
        public string PhoneNumber { get; set; }
    }

    public class VerifyOtpSms_Password
    {
        public string PhoneNumber { get; set; }
        public string OtpCode { get; set; }
    }
}