using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.ODatas
{
    public class ConversationODataDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Email { get; set; }

    }
}
