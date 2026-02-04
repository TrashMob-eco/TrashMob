#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a photo uploaded for an event (before, during, or after).
    /// </summary>
    /// <remarks>
    /// Event photos allow leads and attendees to document cleanup efforts visually.
    /// Photos can be marked as before, during, or after to enable before/after comparisons.
    /// Photo moderation follows the same queue as litter report images and team photos.
    /// </remarks>
    public class EventPhoto : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the associated event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who uploaded the photo.
        /// </summary>
        public Guid UploadedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the URL where the full-size image is stored.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL where the thumbnail image is stored.
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the type of photo (Before, During, After).
        /// </summary>
        public EventPhotoType PhotoType { get; set; } = EventPhotoType.During;

        /// <summary>
        /// Gets or sets an optional caption for the photo.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets when the photo was taken.
        /// </summary>
        public DateTimeOffset? TakenAt { get; set; }

        /// <summary>
        /// Gets or sets the date when the photo was uploaded.
        /// </summary>
        public DateTimeOffset UploadedDate { get; set; }

        /// <summary>
        /// Gets or sets the event this photo belongs to.
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// Gets or sets the user who uploaded the photo.
        /// </summary>
        public virtual User UploadedByUser { get; set; }

        #region Moderation Properties

        /// <summary>
        /// Gets or sets the moderation status (Pending, Approved, Rejected).
        /// </summary>
        public PhotoModerationStatus ModerationStatus { get; set; } = PhotoModerationStatus.Pending;

        /// <summary>
        /// Gets or sets whether the photo is under review (flagged by user, hidden from display).
        /// </summary>
        public bool InReview { get; set; }

        /// <summary>
        /// Gets or sets the user who requested the review (flagged the photo).
        /// </summary>
        public Guid? ReviewRequestedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the review was requested.
        /// </summary>
        public DateTimeOffset? ReviewRequestedDate { get; set; }

        /// <summary>
        /// Gets or sets the moderating admin's user identifier.
        /// </summary>
        public Guid? ModeratedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the photo was moderated.
        /// </summary>
        public DateTimeOffset? ModeratedDate { get; set; }

        /// <summary>
        /// Gets or sets the reason for moderation decision.
        /// </summary>
        public string ModerationReason { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the user who requested review of this photo.
        /// </summary>
        public virtual User ReviewRequestedByUser { get; set; }

        /// <summary>
        /// Gets or sets the admin who moderated this photo.
        /// </summary>
        public virtual User ModeratedByUser { get; set; }
    }
}
