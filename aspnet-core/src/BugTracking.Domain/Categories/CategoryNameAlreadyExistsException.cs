using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace BugTracking.Categories
{
    public class CategoryNameAlreadyExistsException : BusinessException
    {
        public CategoryNameAlreadyExistsException(string name)
            : base(BugTrackingDomainErrorCodes.CategoryNameAlreadyExists)
        {
            WithData("Name", name);
        }
    }
}
