using BugTracking.Issues;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
namespace BugTracking.Categories
{
    public class CategoryDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public List<StatusContractDto> IssueList { get; set; }
        public int count { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatedBy { get; set; }
    }
}
