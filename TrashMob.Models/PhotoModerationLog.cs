#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Audit log entry for photo moderation actions.
    /// </summary>
    public class PhotoModerationLog : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the moderated photo.
        /// </summary>
        public Guid PhotoId { get; set; }

        /// <summary>
        /// Gets or sets the type of photo ("LitterImage" or "TeamPhoto").
        /// </summary>
        public string PhotoType { get; set; }

        /// <summary>
        /// Gets or sets the action taken ("Flagged", "Approved", "Rejected", "FlagDismissed").
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the reason for the action.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who performed the action.
        /// </summary>
        public Guid PerformedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the action was performed.
        /// </summary>
        public DateTimeOffset PerformedDate { get; set; }

        /// <summary>
        /// Gets or sets the user who performed the action.
        /// </summary>
        public virtual User PerformedByUser { get; set; }
    }
}
