namespace BugTracking.Users
{
    public class SendOtpSmsDto
    {
        public string PhoneNumber { get; set; }
    }

    public class SignUpUserDto_VerifyOtpSms
    {
        public string PhoneNumber { get; set; }
        public string OtpCode { get; set; }
    }

    public class SignUpUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string OtpCode { get; set; }
        public string SmsOtpToken { get; set; }
    }
}