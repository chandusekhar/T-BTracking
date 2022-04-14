using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace BugTracking.Statuss
{
     public class StatusAlreadyExistsException : BusinessException
    {
        public StatusAlreadyExistsException(string name)
           : base(BugTrackingDomainErrorCodes.StatusAlreadyExists)
        {
            WithData("name", name);
        }
    }
}
