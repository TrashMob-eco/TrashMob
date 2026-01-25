#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Abstract base model for lookup/reference data entities with name, description, and display properties.
    /// </summary>
    public abstract class LookupModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for this lookup entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of this lookup entry.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this lookup entry.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display order for sorting this lookup entry in lists.
        /// </summary>
        public int? DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this lookup entry is active.
        /// </summary>
        public bool? IsActive { get; set; }
    }
}