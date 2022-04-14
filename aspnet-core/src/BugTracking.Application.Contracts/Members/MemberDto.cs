using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Members
{
    public class MemberDto : EntityDto<Guid>
    {
        public Guid ProjectID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string ProjectName { get; set; }
    }
}
