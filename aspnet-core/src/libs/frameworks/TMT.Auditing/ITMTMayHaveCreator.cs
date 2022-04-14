using JetBrains.Annotations;

namespace TMT.Auditing
{
    public interface ITMTMayHaveCreator<TCreator>
    {
        /// <summary>
        /// Reference to the creator.
        /// </summary>
        [CanBeNull]
        TCreator Creator { get; }
    }

    /// <summary>
    /// Standard interface for an entity that MAY have a creator.
    /// </summary>
    public interface ITMTMayHaveCreator
    {
        /// <summary>
        /// Id of the creator.
        /// </summary>
        string CreatorId { get; }
    }
}