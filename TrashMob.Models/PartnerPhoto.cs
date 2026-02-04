#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a photo in a partner/community photo gallery.
    /// </summary>
    public class PartnerPhoto : KeyedModel
    {
        /// <summary>
        /// Gets or sets the partner identifier.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the URL of the photo.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the photo caption.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the user who uploaded the photo.
        /// </summary>
        public Guid UploadedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the photo was uploaded.
        /// </summary>
        public DateTimeOffset UploadedDate { get; set; }

        #region Moderation Properties

        /// <summary>
        /// Gets or sets the moderation status of the photo.
        /// </summary>
        public PhotoModerationStatus ModerationStatus { get; set; } = PhotoModerationStatus.Pending;

        /// <summary>
        /// Gets or sets whether the photo is currently in review.
        /// </summary>
        public bool InReview { get; set; }

        /// <summary>
        /// Gets or sets the user who requested the review.
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

        #region Navigation Properties

        /// <summary>
        /// Gets or sets the partner this photo belongs to.
        /// </summary>
        public virtual Partner Partner { get; set; }

        /// <summary>
        /// Gets or sets the user who uploaded the photo.
        /// </summary>
        public virtual User UploadedByUser { get; set; }

        /// <summary>
        /// Gets or sets the user who requested review of this photo.
        /// </summary>
        public virtual User ReviewRequestedByUser { get; set; }

        /// <summary>
        /// Gets or sets the admin who moderated this photo.
        /// </summary>
        public virtual User ModeratedByUser { get; set; }

        #endregion
    }
}
