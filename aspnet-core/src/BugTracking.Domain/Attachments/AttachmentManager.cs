using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace BugTracking.Attachments
{
    public class AttachmentManager : DomainService
    {
        public AttachmentManager()
        {
        }
        public Attachment CreateAsync(
            [NotNull] string url,
            [NotNull] Guid tableID
            )
        {
            return new Attachment(
                GuidGenerator.Create(),
                url,
                tableID
            );
        }
    }
}
