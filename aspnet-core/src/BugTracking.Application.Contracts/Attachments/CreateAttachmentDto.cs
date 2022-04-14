using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BugTracking.Attachments
{
    public class CreateAttachmentDto
    {
        public IFormFile File { get; set; }

    }
}
