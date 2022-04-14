using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Categories
{
    public class GetCategoryListDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
