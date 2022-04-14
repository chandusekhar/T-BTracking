using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.DetailAttachment
{
    public class CreateDetailAttachmentDto
    {

        public Guid AttachmentID { get; set; }
        public string Type { get; set; }
        public int Size { get; set; }
        public string Name { get; set; }
    }
}
