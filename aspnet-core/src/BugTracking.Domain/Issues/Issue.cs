using BugTracking.LevelEnum;
using BugTracking.PriorityEnum;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using TMT.Ddd.Domain;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Auditing;

namespace BugTracking.Issues
{
    public class Issue : TMTAuditedAggregateRoot<Guid>, ISoftDelete
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid StatusID { get; set; }
        public Priority Priority { get; set; }
        public Guid? CategoryID { get; set; }
        public DateTime? DueDate { get; set; }
        [AllowNull]
        public Guid ProjectID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public Level IssueLevel { get; set; }
        [DisableAuditing]
        public int CurrentIndex { get; set; }//new field
        public bool IsDeleted { get; set; }
        //[NotMapped]
        public string AuditLogId { get; set; }
        [DisableAuditing]
        public int IdWIT { get; set; }
        [DisableAuditing]
        public int Rev { get; set; }
        public Guid IdParent { get; set; }
        //[NotMapped]
        //[DisableAuditing]
        //public override ExtraPropertyDictionary ExtraProperties { get => base.ExtraProperties; protected set => base.ExtraProperties = value; }
        private Issue()
        {

        }
        internal Issue(
            Guid id,
            [NotNull] string name,
            string description,
            Priority priority,
            Level isselevel,
            Guid? catId,
            Guid projectId,
            Guid stastusId,
            DateTime? dueDate,
            DateTime? startDate,
            DateTime? finishDate,
            int currentIndex,
            int idWIT,
            int rev,
            Guid idParent
            )
            : base(id)
        {
            SetName(name);
            Description = description;
            Priority = priority;
            CategoryID = catId;
            IssueLevel = isselevel;
            ProjectID = projectId;
            StatusID = stastusId;
            DueDate = dueDate;
            StartDate = startDate;
            FinishDate = finishDate;
            CurrentIndex = currentIndex;
            IdWIT = idWIT;
            Rev = rev;
            IdParent = idParent;
        }
        internal Issue ChangeName([NotNull] string issueName)
        {
            SetName(issueName);
            return this;
        }

        private void SetName([NotNull] string issueName)
        {
            Name = Check.NotNullOrWhiteSpace(
                issueName,
                nameof(issueName)
            );
        }
    }
}
