using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.UserInforTfs
{
    public class UserInforTfsDto : EntityDto<Guid>
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string UniqueName { get; set; }
        public string PAT { get; set; }
    }
}
