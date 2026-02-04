namespace TrashMob.Models.Poco
{
    using System;

    /// <summary>
    /// Represents a photo item for the moderation admin interface.
    /// Provides a unified view of LitterImage, TeamPhoto, and EventPhoto entities.
    /// </summary>
    public class PhotoModerationItem
    {
        /// <summary>
        /// Gets or sets the photo identifier.
        /// </summary>
        public Guid PhotoId { get; set; }

        /// <summary>
        /// Gets or sets the type of photo ("LitterImage", "TeamPhoto", or "EventPhoto").
        /// </summary>
        public string PhotoType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL where the image is stored.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the moderation status.
        /// </summary>
        public PhotoModerationStatus ModerationStatus { get; set; }

        /// <summary>
        /// Gets or sets whether the photo is currently under review (flagged).
        /// </summary>
        public bool InReview { get; set; }

        /// <summary>
        /// Gets or sets when the photo was flagged for review.
        /// </summary>
        public DateTimeOffset? FlaggedDate { get; set; }

        /// <summary>
        /// Gets or sets the reason the photo was flagged.
        /// </summary>
        public string? FlagReason { get; set; }

        /// <summary>
        /// Gets or sets when the photo was uploaded.
        /// </summary>
        public DateTimeOffset UploadedDate { get; set; }

        /// <summary>
        /// Gets or sets the user ID who uploaded the photo.
        /// </summary>
        public Guid UploadedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the username of who uploaded the photo.
        /// </summary>
        public string? UploaderName { get; set; }

        /// <summary>
        /// Gets or sets the uploader's email address (for notification purposes).
        /// </summary>
        public string? UploaderEmail { get; set; }

        /// <summary>
        /// Gets or sets the litter report ID (if this is a LitterImage).
        /// </summary>
        public Guid? LitterReportId { get; set; }

        /// <summary>
        /// Gets or sets the litter report name (if this is a LitterImage).
        /// </summary>
        public string? LitterReportName { get; set; }

        /// <summary>
        /// Gets or sets the team ID (if this is a TeamPhoto).
        /// </summary>
        public Guid? TeamId { get; set; }

        /// <summary>
        /// Gets or sets the team name (if this is a TeamPhoto).
        /// </summary>
        public string? TeamName { get; set; }

        /// <summary>
        /// Gets or sets the event ID (if this is an EventPhoto).
        /// </summary>
        public Guid? EventId { get; set; }

        /// <summary>
        /// Gets or sets the event name (if this is an EventPhoto).
        /// </summary>
        public string? EventName { get; set; }

        /// <summary>
        /// Gets or sets the event photo type (Before, During, After - only for EventPhoto).
        /// </summary>
        public EventPhotoType? EventPhotoTypeValue { get; set; }

        /// <summary>
        /// Gets or sets the caption (for TeamPhoto or EventPhoto).
        /// </summary>
        public string? Caption { get; set; }

        /// <summary>
        /// Gets or sets when the photo was moderated.
        /// </summary>
        public DateTimeOffset? ModeratedDate { get; set; }

        /// <summary>
        /// Gets or sets who moderated the photo.
        /// </summary>
        public string? ModeratedByName { get; set; }

        /// <summary>
        /// Gets or sets the moderation reason.
        /// </summary>
        public string? ModerationReason { get; set; }
    }
}
