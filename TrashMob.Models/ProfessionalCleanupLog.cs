#nullable disable

namespace TrashMob.Models
{
    using System;

    /// <summary>
    /// Records a professional cleanup performed by a company on a sponsored adoptable area.
    /// This data is completely separate from volunteer events and does NOT count toward
    /// volunteer leaderboards, impact stats, or gamification.
    /// </summary>
    public class ProfessionalCleanupLog : KeyedModel
    {
        /// <summary>
        /// Gets or sets the sponsored adoption this cleanup is for.
        /// </summary>
        public Guid SponsoredAdoptionId { get; set; }

        /// <summary>
        /// Gets or sets the professional company that performed the cleanup.
        /// </summary>
        public Guid ProfessionalCompanyId { get; set; }

        /// <summary>
        /// Gets or sets the date the cleanup was performed.
        /// </summary>
        public DateTimeOffset CleanupDate { get; set; }

        /// <summary>
        /// Gets or sets the duration of the cleanup in minutes.
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the number of bags collected.
        /// </summary>
        public int BagsCollected { get; set; }

        /// <summary>
        /// Gets or sets the weight of trash collected in pounds (optional).
        /// </summary>
        public decimal? WeightInPounds { get; set; }

        /// <summary>
        /// Gets or sets the weight of trash collected in kilograms (optional).
        /// </summary>
        public decimal? WeightInKilograms { get; set; }

        /// <summary>
        /// Gets or sets notes about the cleanup.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the sponsored adoption navigation property.
        /// </summary>
        public virtual SponsoredAdoption SponsoredAdoption { get; set; }

        /// <summary>
        /// Gets or sets the professional company navigation property.
        /// </summary>
        public virtual ProfessionalCompany ProfessionalCompany { get; set; }
    }
}
