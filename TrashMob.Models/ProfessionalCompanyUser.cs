#nullable disable

namespace TrashMob.Models
{
    using System;

    /// <summary>
    /// Represents a user who is associated with a professional cleanup company.
    /// Uses a composite key of ProfessionalCompanyId and UserId (like PartnerAdmin).
    /// </summary>
    public class ProfessionalCompanyUser : BaseModel
    {
        /// <summary>
        /// Gets or sets the professional company identifier.
        /// </summary>
        public Guid ProfessionalCompanyId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the professional company navigation property.
        /// </summary>
        public virtual ProfessionalCompany ProfessionalCompany { get; set; }

        /// <summary>
        /// Gets or sets the user navigation property.
        /// </summary>
        public virtual User User { get; set; }
    }
}
