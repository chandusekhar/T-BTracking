using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Attachments
{
    public class GetAttachmentListDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }//
    }
}
