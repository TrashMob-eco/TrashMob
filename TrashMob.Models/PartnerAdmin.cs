#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents an administrator relationship between a user and a partner organization.
    /// </summary>
    public class PartnerAdmin : BaseModel
    {
        /// <summary>
        /// Gets or sets the identifier of the partner organization.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who is an administrator.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the partner organization.
        /// </summary>
        public virtual Partner Partner { get; set; }

        /// <summary>
        /// Gets or sets the user who is an administrator.
        /// </summary>
        public virtual User User { get; set; }
    }
}