#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a sponsored adoption where a sponsor pays for a professional company to maintain an adoptable area.
    /// </summary>
    public class SponsoredAdoption : KeyedModel
    {
        public SponsoredAdoption()
        {
            CleanupLogs = [];
        }

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
        public int CleanupFrequencyDays { get; set; } = 14;

        /// <summary>
        /// Gets or sets the status of this sponsored adoption (Active, Expired, Terminated).
        /// </summary>
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Gets or sets the adoptable area navigation property.
        /// </summary>
        public virtual AdoptableArea AdoptableArea { get; set; }

        /// <summary>
        /// Gets or sets the sponsor navigation property.
        /// </summary>
        public virtual Sponsor Sponsor { get; set; }

        /// <summary>
        /// Gets or sets the professional company navigation property.
        /// </summary>
        public virtual ProfessionalCompany ProfessionalCompany { get; set; }

        /// <summary>
        /// Gets or sets the collection of cleanup logs for this sponsored adoption.
        /// </summary>
        public virtual ICollection<ProfessionalCleanupLog> CleanupLogs { get; set; }
    }
}
