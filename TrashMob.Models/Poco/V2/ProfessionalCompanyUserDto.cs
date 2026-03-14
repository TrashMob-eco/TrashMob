#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a professional company user association. Composite key DTO (no Id).
    /// </summary>
    public class ProfessionalCompanyUserDto
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
        /// Gets or sets when the association was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
