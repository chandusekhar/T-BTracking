using BugTracking.Departments;
using BugTracking.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Conversation
{
    public class InfoUserDto
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string teams { get; set; }
        public string departments { get; set; }
    }
}
