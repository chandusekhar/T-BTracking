using System;
using TMT.Auditing;

namespace TMT.Ddd.Domain
{
    /// <summary>
    /// Implements <see cref="ITMTFullAuditedObject"/> to be a base class for full-audited aggregate roots.
    /// </summary>
    [Serializable]
    public abstract class TMTFullAuditedAggregateRoot : TMTAuditedAggregateRoot, ITMTFullAuditedObject
    {
        /// <inheritdoc />
        public virtual bool IsDeleted { get; set; }

        /// <inheritdoc />
        public virtual string DeleterId { get; set; }

        /// <inheritdoc />
        public virtual DateTime? DeletionTime { get; set; }
    }

    /// <summary>
    /// Implements <see cref="ITMTFullAuditedObject"/> to be a base class for full-audited aggregate roots.
    /// </summary>
    /// <typeparam name="TKey">Type of the primary key of the entity</typeparam>
    [Serializable]
    public abstract class TMTFullAuditedAggregateRoot<TKey> : TMTAuditedAggregateRoot<TKey>, ITMTFullAuditedObject
    {
        /// <inheritdoc />
        public virtual bool IsDeleted { get; set; }

        /// <inheritdoc />
        public virtual string DeleterId { get; set; }

        /// <inheritdoc />
        public virtual DateTime? DeletionTime { get; set; }

        protected TMTFullAuditedAggregateRoot()
        {
        }

        protected TMTFullAuditedAggregateRoot(TKey id)
        : base(id)
        {
        }
    }
}