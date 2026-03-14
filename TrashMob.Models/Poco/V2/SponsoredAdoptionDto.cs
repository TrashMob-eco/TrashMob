#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a sponsored adoption. Flat DTO excluding navigation properties.
    /// </summary>
    public class SponsoredAdoptionDto
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the adoptable area being sponsored.
        /// </summary>
        public Guid AdoptableAreaId { get; set; }

        /// <summary>
        /// Gets or sets the sponsor funding this adoption.
        /// </summary>
        public Guid SponsorId { get; set; }

        /// <summary>
        /// Gets or sets the professional company servicing this area.
        /// </summary>
        public Guid ProfessionalCompanyId { get; set; }

        /// <summary>
        /// Gets or sets the start date of the sponsored adoption.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the sponsored adoption (null for open-ended).
        /// </summary>
        public DateOnly? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the expected cleanup frequency in days.
        /// </summary>
        public int CleanupFrequencyDays { get; set; }

        /// <summary>
        /// Gets or sets the status (Active, Expired, Terminated).
        /// </summary>
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Gets or sets when the sponsored adoption was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
