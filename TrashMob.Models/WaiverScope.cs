namespace TrashMob.Models
{
    /// <summary>
    /// Defines the scope of a waiver.
    /// </summary>
    public enum WaiverScope
    {
        /// <summary>
        /// Global waiver that applies to all events (e.g., TrashMob platform waiver).
        /// </summary>
        Global = 0,

        /// <summary>
        /// Community-specific waiver that requires assignment to a community.
        /// </summary>
        Community = 1
    }
}
