using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace BugTracking.Categories
{
    public class Category : TMTAuditedAggregateRoot<Guid>, ISoftDelete
    {
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        private Category()
        {
            /* This constructor is for deserialization / ORM purpose */
        }
        internal Category(
                Guid id,
                [NotNull] string name)
            : base(id)
        {
            Name = name;
        }
    }
   
}
