using BugTracking.Assignees;
using BugTracking.Attachments;
using BugTracking.LevelEnum;
using BugTracking.PriorityEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace BugTracking.Issues
{
    public class IssuesDto : EntityDto<Guid>
    {
      //  public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid StatusID { get; set; }
        public string StatusName { get; set; }
        public Priority Priority { get; set; }//
        public Guid? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public DateTime? DueDate { get; set; }
        public int CountIssueDueDate { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserNameAssign { get; set; }
        public Guid ProjectID { get; set; }
        public string ProjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime CreationTime { get; set; }
        public string  PriorityValue { get; set; }
        public string LevelValue { get; set; }
        public DateTime? FinishDate { get; set; }
        public Level IssueLevel { get; set; }
        public int CurrentIndex { get; set; }
        public string NzColor { get; set; }
        public string CreatorId { get; set; }
        public int DueInDay { get; set; }
        public int DueFull { get; set; }
        public int follows { get; set; }
        public int comments { get; set; }
        public int attachments { get; set; }
        public List<string> ListUserAssign { get; set; }
        public List<AssigneeDto> AssigneesList { get; set; }
        public List<AttachmentDto> AttachmentListImage { get; set; }
        public List<AttachmentDto> AttachmentListVideo { get; set; }
        public PagedResultDto<IssuesDto> IssuesChildDto { get; set; }
        public IssuesDto IssueParentDto { get; set; }
        public int IdWIT { get; set; }
        public Guid IdParent { get; set; }
        public bool IsHaveParent { get; set; }
    }
}
