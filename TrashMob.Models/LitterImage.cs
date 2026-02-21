#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents an image associated with a litter report.
    /// </summary>
    public class LitterImage : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the associated litter report.
        /// </summary>
        public Guid LitterReportId { get; set; }

        /// <summary>
        /// Gets or sets the Azure Blob Storage URL where the image is stored.
        /// </summary>
        public string AzureBlobURL { get; set; }

        /// <summary>
        /// Gets or sets the street address where the litter was photographed.
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the city where the litter was photographed.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the region or state where the litter was photographed.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the country where the litter was photographed.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the litter location.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the latitude coordinate of the litter location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate of the litter location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this litter image has been cancelled.
        /// </summary>
        public bool IsCancelled { get; set; }

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
        /// Gets or sets the litter report this image belongs to.
        /// </summary>
        public virtual LitterReport LitterReport { get; set; }

        /// <summary>
        /// Gets or sets the user who requested review of this image.
        /// </summary>
        public virtual User ReviewRequestedByUser { get; set; }

        /// <summary>
        /// Gets or sets the admin who moderated this image.
        /// </summary>
        public virtual User ModeratedByUser { get; set; }
    }
}