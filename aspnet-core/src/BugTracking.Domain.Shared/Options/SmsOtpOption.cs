using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracking.Options
{
    public class SmsOtpOption
    {
        public TimeSpan CodeVerifyLifeTime { get; set; }
        public TimeSpan TokenAfterVerifyLifeTime { get; set; }
        public string SecretSalt { get; set; }
    }
}
