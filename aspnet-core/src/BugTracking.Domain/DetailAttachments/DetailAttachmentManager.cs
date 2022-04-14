using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace BugTracking.DetailAttachments
{
    public class DetailAttachmentManager : DomainService
    {
        private readonly IDetailAttachmentRepository _detailattachmentRepository;
        public DetailAttachmentManager(IDetailAttachmentRepository detailattachmentRepository)
        {
            _detailattachmentRepository = detailattachmentRepository;
        }
        public DetailAttachment CreateAsync(
            [NotNull] Guid attachmentID,
             string type,
             int size,
             string fileName
            )
        {
            return new DetailAttachment(
                GuidGenerator.Create(),
                attachmentID,
                type,
                size,
                fileName
            );
        }
    }
}
