using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Statuss
{
     public class GetStatusListDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
