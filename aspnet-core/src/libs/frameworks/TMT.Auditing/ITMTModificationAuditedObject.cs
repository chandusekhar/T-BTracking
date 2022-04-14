using Volo.Abp.Auditing;

namespace TMT.Auditing
{
    /// <summary>
    /// This interface can be implemented to store modification information (who and when modified lastly).
    /// </summary>
    public interface ITMTModificationAuditedObject : IHasModificationTime
    {
        /// <summary>
        /// Last modifier user for this entity.
        /// </summary>
        string LastModifierId { get; set; }
    }

    /// <summary>
    /// Adds navigation properties to <see cref="ITMTModificationAuditedObject"/> interface for a user.
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface ITMTModificationAuditedObject<TUser> : ITMTModificationAuditedObject
    {
        /// <summary>
        /// Reference to the last modifier user of this entity.
        /// </summary>
        TUser LastModifier { get; set; }
    }
}