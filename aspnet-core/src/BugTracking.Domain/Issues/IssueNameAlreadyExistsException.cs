using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace BugTracking.Issues
{
    public class IssueNameAlreadyExistsException : BusinessException
    {
        public IssueNameAlreadyExistsException(string name) : base(BugTrackingDomainErrorCodes.IssueNameAlreadyExists)
        {
            WithData("Name", name);
        }
    }
}
