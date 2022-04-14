using Volo.Abp.Auditing;

namespace TMT.Auditing
{
    /// <summary>
    /// This interface can be implemented to store deletion information (who delete and when deleted).
    /// </summary>
    public interface ITMTDeletionAuditedObject : IHasDeletionTime
    {
        /// <summary>
        /// Id of the deleter user.
        /// </summary>
        string DeleterId { get; set; }
    }

    /// <summary>
    /// Extends <see cref="ITMTDeletionAuditedObject"/> to add user navigation propery.
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface ITMTDeletionAuditedObject<TUser> : ITMTDeletionAuditedObject
    {
        /// <summary>
        /// Reference to the deleter user.
        /// </summary>
        TUser Deleter { get; set; }
    }
}