using Volo.Abp.Auditing;

namespace TMT.Auditing
{
    /// <summary>
    /// This interface can be implemented to store creation information (who and when created).
    /// </summary>
    public interface ITMTCreationAuditedObject : IHasCreationTime, ITMTMayHaveCreator
    {
    }

    /// <summary>
    /// Adds navigation property (object reference) to <see cref="ITMTCreationAuditedObject"/> interface.
    /// </summary>
    /// <typeparam name="TCreator">Type of the user</typeparam>
    public interface ITMTCreationAuditedObject<TCreator> : ITMTCreationAuditedObject, ITMTMayHaveCreator<TCreator>
    {
    }
}