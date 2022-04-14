using System;
using TMT.Auditing;

namespace TMT.Ddd.Domain
{
    /// <summary>
    /// This class can be used to simplify implementing <see cref="ITMTAuditedObject"/> for aggregate roots.
    /// </summary>
    [Serializable]
    public abstract class TMTAuditedAggregateRoot : TMTCreationAuditedAggregateRoot, ITMTAuditedObject
    {
        /// <inheritdoc />
        public virtual DateTime? LastModificationTime { get; set; }

        /// <inheritdoc />
        public virtual string LastModifierId { get; set; }
    }

    /// <summary>
    /// This class can be used to simplify implementing <see cref="ITMTAuditedObject"/> for aggregate roots.
    /// </summary>
    /// <typeparam name="TKey">Type of the primary key of the entity</typeparam>
    [Serializable]
    public abstract class TMTAuditedAggregateRoot<TKey> : TMTCreationAuditedAggregateRoot<TKey>, ITMTAuditedObject
    {
        /// <inheritdoc />
        public virtual DateTime? LastModificationTime { get; set; }

        /// <inheritdoc />
        public virtual string LastModifierId { get; set; }

        protected TMTAuditedAggregateRoot()
        {
        }

        protected TMTAuditedAggregateRoot(TKey id)
            : base(id)
        {
        }
    }
}