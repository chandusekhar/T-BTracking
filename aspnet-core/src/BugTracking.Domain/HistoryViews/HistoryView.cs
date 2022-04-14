using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace BugTracking.HistoryViews
{
    public class HistoryView : Entity
    {
        public string UserName { get; set; }
        public string HttpMethod { get; set; }
        public DateTime ExecutionTime { get; set; }
        public string NewValue { get; set; }
        public string OriginalValue { get; set; }
        public string EntityId { get; set; }
        public string UserId { get; set; }
        public Guid Id { get; set; }

        public override object[] GetKeys()
        {
            return null;
        }
    }
}
