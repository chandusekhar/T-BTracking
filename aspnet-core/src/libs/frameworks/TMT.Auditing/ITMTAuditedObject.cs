namespace TMT.Auditing
{
    /// <summary>
    /// This interface can be implemented to add standard auditing properties to a class.
    /// </summary>
    public interface ITMTAuditedObject : ITMTCreationAuditedObject, ITMTModificationAuditedObject
    {
    }

    /// <summary>
    /// Extends <see cref="ITMTAuditedObject"/> to add user navigation properties.
    /// </summary>
    /// <typeparam name="TUser">Type of the user</typeparam>
    public interface ITMTAuditedObject<TUser> : ITMTAuditedObject, ITMTCreationAuditedObject<TUser>, ITMTModificationAuditedObject<TUser>
    {
    }
}