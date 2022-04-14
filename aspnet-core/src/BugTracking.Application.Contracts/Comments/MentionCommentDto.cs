using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Comments
{
    public class MentionCommentDto
    {
        public string id { get; set; }
        public string value { get; set; }
    }
}
