using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Projects
{
    public class UserProcessingDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Bugs { get; set; }
        public string BugsName { get; set; }
    }
}
