using BugTracking.DetailAttachment;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Attachments
{
    public class AttachmentDto : EntityDto<Guid>
    {
        public string URL { get; set; }
        public Guid TableID { get; set; }//
        public string Name { get; set; }
        public string Type { get; set; }
        public int Size { get; set; }
    }
}
