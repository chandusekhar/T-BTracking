namespace TMT.Auditing
{
    /// <summary>
    /// This interface adds <see cref="ITMTDeletionAuditedObject"/> to <see cref="ITMTAuditedObject"/>.
    /// </summary>
    public interface ITMTFullAuditedObject : ITMTAuditedObject, ITMTDeletionAuditedObject
    {
    }

    /// <summary>
    /// Adds user navigation properties to <see cref="ITMTFullAuditedObject"/> interface for user.
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface ITMTFullAuditedObject<TUser> : ITMTAuditedObject<TUser>, ITMTFullAuditedObject, ITMTDeletionAuditedObject<TUser>
    {
    }
}