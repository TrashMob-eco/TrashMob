#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a photo in a team's photo gallery.
    /// </summary>
    /// <remarks>
    /// Teams can have a photo gallery to showcase their cleanup efforts.
    /// Photos are uploaded by team members and can include captions.
    /// Photo moderation follows the same queue as litter report images.
    /// </remarks>
    public class TeamPhoto : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the team this photo belongs to.
        /// </summary>
        public Guid TeamId { get; set; }

        /// <summary>
        /// Gets or sets the URL where the image is stored.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets an optional caption for the photo.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who uploaded the photo.
        /// </summary>
        public Guid UploadedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date when the photo was uploaded.
        /// </summary>
        public DateTimeOffset UploadedDate { get; set; }

        /// <summary>
        /// Gets or sets the team this photo belongs to.
        /// </summary>
        public virtual Team Team { get; set; }

        /// <summary>
        /// Gets or sets the user who uploaded the photo.
        /// </summary>
        public virtual User UploadedByUser { get; set; }
    }
}
