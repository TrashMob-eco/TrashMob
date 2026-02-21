namespace TrashMob.Models
{
    /// <summary>
    /// Represents the moderation status of a user-uploaded photo.
    /// </summary>
    public enum PhotoModerationStatus
    {
        /// <summary>
        /// Photo is newly uploaded and has not been reviewed.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Photo has been reviewed and approved for display.
        /// </summary>
        Approved = 1,

        /// <summary>
        /// Photo has been reviewed and removed for policy violation.
        /// </summary>
        Rejected = 2
    }
}
