using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.TimeOnProjects
{
    public class TimeOnProjectDto 
    {
        //public string ApplicationName { get; set; }
        //public string UrlPath { get; set; }
        //public bool IsActive { get; set; } = true;
        //public DateTime StartTime { get; set; }
        //public DateTime EndTime { get; set; }
        public int Id { get; set; }
        public Guid Idproject { get; set; }
        public string NameProject { get; set; }
        public string IdUser { get; set; }
        public string NameUser { get; set; }
        public string UniqueName { get; set; }
        public SumApplicationDto Application { get; set; }
    }
}
