
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace BugTracking.Projects
{
    [Index(nameof(WitType))]
    public class Project : TMTAuditedAggregateRoot<Guid>, ISoftDelete
    {

        public string Name { get; set; }
        public string NzColor { get; set; }
        [DisableAuditing]
        public Guid ProjectIdTFS { get; set; }
        [DisableAuditing]
        [MaxLength(30)]
        public string WitType { get; set; }
        public bool IsDeleted { get; set; }
        private Project()
        {
            /* This constructor is for deserialization / ORM purpose */
        }
        public Project(Guid id,
            [NotNull] string name,
             string nzColor,
             Guid projectIdTFS,
             string witType

            ) : base(id)
        {
            Name = name;
            NzColor = nzColor;
            ProjectIdTFS=projectIdTFS;
            WitType = witType;
        }

        internal Project ChangeName([NotNull] string projectName)
        {
            SetName(projectName);
            return this;
        }
        private void SetName([NotNull] string name)
        {
            Name = Check.NotNullOrWhiteSpace(
                name,
                nameof(name)
            );
        }
    }
}
