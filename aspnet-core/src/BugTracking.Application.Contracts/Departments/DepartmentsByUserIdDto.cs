using BugTracking.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Departments
{
    public class DepartmentsByUserIdDTO
    {
        public string DeparmentName { get; set; }
        public List<TeamDto> ListTeam { get; set; }
    }
}
