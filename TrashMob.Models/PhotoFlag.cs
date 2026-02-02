#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a user flag/report of an inappropriate photo.
    /// </summary>
    public class PhotoFlag : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the flagged photo.
        /// </summary>
        public Guid PhotoId { get; set; }

        /// <summary>
        /// Gets or sets the type of photo ("LitterImage" or "TeamPhoto").
        /// </summary>
        public string PhotoType { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who flagged the photo.
        /// </summary>
        public Guid FlaggedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the reason for flagging the photo.
        /// </summary>
        public string FlagReason { get; set; }

        /// <summary>
        /// Gets or sets when the photo was flagged.
        /// </summary>
        public DateTimeOffset FlaggedDate { get; set; }

        /// <summary>
        /// Gets or sets when the flag was resolved by an admin.
        /// </summary>
        public DateTimeOffset? ResolvedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the admin who resolved the flag.
        /// </summary>
        public Guid? ResolvedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the resolution outcome ("Approved", "Rejected", "Dismissed").
        /// </summary>
        public string Resolution { get; set; }

        /// <summary>
        /// Gets or sets the user who flagged the photo.
        /// </summary>
        public virtual User FlaggedByUser { get; set; }

        /// <summary>
        /// Gets or sets the admin who resolved the flag.
        /// </summary>
        public virtual User ResolvedByUser { get; set; }
    }
}
