using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace BugTracking.Projects
{
    public class ProjectNameAlreadyExistsException : BusinessException
    {
        public ProjectNameAlreadyExistsException(string name) 
        {

        }
    }
}
