using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp.Domain.Entities.Auditing;

namespace BugTracking.DetailAttachments
{
    public class DetailAttachment : TMTAuditedAggregateRoot<Guid>
    {
        public Guid AttachmentID { get; set; }
        public string Type { get; set; }
        public int Size { get; set; } 
        public string FileName { get; set; }
        private DetailAttachment()
        {
        }

        internal DetailAttachment(
            Guid id,
            [NotNull] Guid attachmentID,
             string type,
             int size,
             string fileName
            ) : base(id)
        {
            AttachmentID = attachmentID;
            Type = type;
            Size = size;
            FileName = fileName;
        }
    }
}
