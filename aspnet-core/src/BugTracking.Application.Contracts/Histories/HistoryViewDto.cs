using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Histories
{
    public class HistoryViewDto : EntityDto<Guid>
    {
        public string UserName { get; set; }
        public string HttpMethod { get; set; }
        public DateTime ExecutionTime { get; set; }
        public string NewValue { get; set; }
        public string OriginalValue { get; set; }
        public string EntityId { get; set; }
        public string UserId { get; set; }
    }
}
