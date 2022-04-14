using System;
using TMT.Auditing;
using Volo.Abp.Domain.Entities;

namespace TMT.Ddd.Domain
{
    /// <summary>
    /// This class can be used to simplify implementing <see cref="ITMTCreationAuditedObject"/> for aggregate roots.
    /// </summary>
    [Serializable]
    public abstract class TMTCreationAuditedAggregateRoot : AggregateRoot, ITMTCreationAuditedObject
    {
        /// <inheritdoc />
        public virtual DateTime CreationTime { get; protected set; }

        /// <inheritdoc />
        public virtual string CreatorId { get; protected set; }
    }

    /// <summary>
    /// This class can be used to simplify implementing <see cref="ITMTCreationAuditedObject"/> for aggregate roots.
    /// </summary>
    /// <typeparam name="TKey">Type of the primary key of the entity</typeparam>
    [Serializable]
    public abstract class TMTCreationAuditedAggregateRoot<TKey> : AggregateRoot<TKey>, ITMTCreationAuditedObject
    {
        /// <inheritdoc />
        public virtual DateTime CreationTime { get; set; }

        /// <inheritdoc />
        public virtual string CreatorId { get; set; }

        protected TMTCreationAuditedAggregateRoot()
        {
        }

        protected TMTCreationAuditedAggregateRoot(TKey id)
            : base(id)
        {
        }
    }
}