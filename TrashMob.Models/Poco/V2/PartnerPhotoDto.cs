#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a partner/community photo. Flat DTO excluding navigation and moderation properties.
    /// </summary>
    public class PartnerPhotoDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the photo.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the partner identifier this photo belongs to.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the URL where the image is stored.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the caption for the photo.
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identifier of the user who uploaded the photo.
        /// </summary>
        public Guid UploadedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date when the photo was uploaded.
        /// </summary>
        public DateTimeOffset UploadedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created this record.
        /// </summary>
        public Guid CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when this record was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets when this record was last updated.
        /// </summary>
        public DateTimeOffset LastUpdatedDate { get; set; }
    }
}
