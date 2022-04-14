using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Projects
{
    public class UserProcessingDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Bugs { get; set; }
        public string Projects { get; set; }
        public List<UserProcessingDetailDto> userProcessingDetailDtos { get; set; }
    }
}
