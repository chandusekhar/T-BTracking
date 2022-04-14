using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Follows
{
    public class GetFollowDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
