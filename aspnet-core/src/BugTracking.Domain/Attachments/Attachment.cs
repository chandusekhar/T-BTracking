using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp.Domain.Entities.Auditing;

namespace BugTracking.Attachments
{
   public class Attachment : TMTAuditedAggregateRoot<Guid>
    {
        public Guid TableID { get; set; }
        public string URL { get; set; }
        private Attachment()
        {
        }

        internal Attachment(
            Guid id,
            [NotNull] string url,
            [NotNull] Guid idTable
            ) : base(id)
        {
            URL = url;
            TableID = idTable;
        }
    }
}
