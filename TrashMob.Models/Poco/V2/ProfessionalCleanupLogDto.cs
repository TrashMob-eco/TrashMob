#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a professional cleanup log entry.
    /// </summary>
    public class ProfessionalCleanupLogDto
    {
        /// <summary>Gets or sets the unique identifier.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the sponsored adoption this cleanup is for.</summary>
        public Guid SponsoredAdoptionId { get; set; }

        /// <summary>Gets or sets the professional company that performed the cleanup.</summary>
        public Guid ProfessionalCompanyId { get; set; }

        /// <summary>Gets or sets the date the cleanup was performed.</summary>
        public DateTimeOffset CleanupDate { get; set; }

        /// <summary>Gets or sets the duration of the cleanup in minutes.</summary>
        public int DurationMinutes { get; set; }

        /// <summary>Gets or sets the number of bags collected.</summary>
        public int BagsCollected { get; set; }

        /// <summary>Gets or sets the weight of trash collected in pounds.</summary>
        public decimal? WeightInPounds { get; set; }

        /// <summary>Gets or sets the weight of trash collected in kilograms.</summary>
        public decimal? WeightInKilograms { get; set; }

        /// <summary>Gets or sets notes about the cleanup.</summary>
        public string? Notes { get; set; }

        /// <summary>Gets or sets when the log was created.</summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
