#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Abstract base model providing common audit fields for tracking creation and modification.
    /// </summary>
    public abstract class BaseModel
    {
        /// <summary>
        /// Gets or sets the identifier of the user who created this entity.
        /// </summary>
        public Guid CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this entity was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last updated this entity.
        /// </summary>
        public Guid LastUpdatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this entity was last updated.
        /// </summary>
        public DateTimeOffset? LastUpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user who created this entity.
        /// </summary>
        public virtual User CreatedByUser { get; set; }

        /// <summary>
        /// Gets or sets the user who last updated this entity.
        /// </summary>
        public virtual User LastUpdatedByUser { get; set; }
    }
}