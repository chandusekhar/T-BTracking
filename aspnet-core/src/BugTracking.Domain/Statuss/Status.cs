using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace BugTracking.Statuss
{
    public class Status : TMTAuditedAggregateRoot<Guid>, ISoftDelete
    {
        public string Name { get; set; }
        public int CurrentIndex { get; set; }
        public bool IsDefault { get; set; }
        public string NzColor { get; set; }
        public bool IsDeleted { get ; set ; }

        private Status()
        {
            /* This constructor is for deserialization / ORM purpose */
        }

        internal Status(
            Guid id,
            [NotNull] string name,
            int currentIndex,
            bool isDefault,
            string nzColor
            )
            : base(id)
        {
            SetName(name);
            CurrentIndex = currentIndex;
            IsDefault = isDefault;
            NzColor = nzColor;
        }

        internal Status ChangeName([NotNull] string name)
        {
            SetName(name);
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

