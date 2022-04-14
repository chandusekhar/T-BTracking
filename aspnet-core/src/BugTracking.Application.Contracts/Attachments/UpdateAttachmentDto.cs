using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Attachments
{
    public class UpdateAttachmentDto
    {
        public IFormFile File { get; set; }//
    }
}
