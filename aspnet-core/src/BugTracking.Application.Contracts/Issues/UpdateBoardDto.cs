using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Issues
{
    public class UpdateBoardDto
    {
        public Container Container { get; set; }
        public int Index { get; set; }
        public PreviousContainer PreviousContainer { get; set; }
        public int PreviousIndex { get; set; }
    }

    public class Container
    {
        public Guid StatusId { get; set; }
        public List<Guid> IssuesId { get; set; }
    }
    public class PreviousContainer
    {
        public List<Guid> IssuesId { get; set; }
    }
}
