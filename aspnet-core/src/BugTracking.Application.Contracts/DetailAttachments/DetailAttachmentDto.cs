using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.DetailAttachment
{
    public class DetailAttachmentDto : EntityDto<Guid>
    {
        public Guid AttachmentID { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string FileName { get; set; }
    }
}
