using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.Ddd.Domain;
using Volo.Abp;

namespace BugTracking.Azures
{
    public class Azure : TMTAuditedAggregateRoot<Guid>, ISoftDelete
    {
        public string Host { get; set; }
        public string Collection { get; set; }
        public bool IsDeleted { get ; set; }
        private Azure()
        {
            /* This constructor is for deserialization / ORM purpose */
        }
        internal Azure(
                Guid id,
                [NotNull] string host,
                [NotNull] string collection
                )
            : base(id)
        {
            Host = host;
            Collection = collection;
        }
    }

}
