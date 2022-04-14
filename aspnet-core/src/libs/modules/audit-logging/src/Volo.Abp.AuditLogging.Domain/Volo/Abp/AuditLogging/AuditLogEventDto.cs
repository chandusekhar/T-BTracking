using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;

namespace Volo.Abp.AuditLogging
{
    public class AuditLogEventDto
    {
        public string IssueId { get; set; }
        public string AuditLogId { get; set; }
    }
}
