using JetBrains.Annotations;

namespace TMT.Auditing
{
    public interface ITMTMustHaveCreator<TCreator> : ITMTMustHaveCreator
    {
        /// <summary>
        /// Reference to the creator.
        /// </summary>
        [NotNull]
        TCreator Creator { get; }
    }

    /// <summary>
    /// Standard interface for an entity that MUST have a creator.
    /// </summary>
    public interface ITMTMustHaveCreator
    {
        /// <summary>
        /// Id of the creator.
        /// </summary>
        ///
        [NotNull]
        string CreatorId { get; }
    }
}