#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Abstract base model that extends <see cref="BaseModel"/> with a unique identifier.
    /// </summary>
    public abstract class KeyedModel : BaseModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for this entity.
        /// </summary>
        public Guid Id { get; set; }
    }
}