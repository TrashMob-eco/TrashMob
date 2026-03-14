#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of an adoptable area. Flat DTO excluding navigation properties.
    /// </summary>
    public class AdoptableAreaDto
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the community (partner) this area belongs to.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the area name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the area description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the type of area (Highway, Park, School, Trail, Waterway, Street, Spot).
        /// </summary>
        public string? AreaType { get; set; }

        /// <summary>
        /// Gets or sets the adoption status (Available, Adopted, Unavailable).
        /// </summary>
        public string Status { get; set; } = "Available";

        /// <summary>
        /// Gets or sets the GeoJSON representation of the area boundary/route.
        /// </summary>
        public string? GeoJson { get; set; }

        /// <summary>
        /// Gets or sets the start point latitude (for linear areas).
        /// </summary>
        public double? StartLatitude { get; set; }

        /// <summary>
        /// Gets or sets the start point longitude (for linear areas).
        /// </summary>
        public double? StartLongitude { get; set; }

        /// <summary>
        /// Gets or sets the end point latitude (for linear areas).
        /// </summary>
        public double? EndLatitude { get; set; }

        /// <summary>
        /// Gets or sets the end point longitude (for linear areas).
        /// </summary>
        public double? EndLongitude { get; set; }

        /// <summary>
        /// Gets or sets how often cleanup is required (in days).
        /// </summary>
        public int CleanupFrequencyDays { get; set; }

        /// <summary>
        /// Gets or sets the minimum events required per year.
        /// </summary>
        public int MinEventsPerYear { get; set; }

        /// <summary>
        /// Gets or sets safety requirements and guidelines.
        /// </summary>
        public string? SafetyRequirements { get; set; }

        /// <summary>
        /// Gets or sets whether this area can be co-adopted by multiple teams.
        /// </summary>
        public bool AllowCoAdoption { get; set; }

        /// <summary>
        /// Gets or sets whether this area is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created this area.
        /// </summary>
        public Guid CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the area was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last updated this area.
        /// </summary>
        public Guid LastUpdatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the area was last updated.
        /// </summary>
        public DateTimeOffset? LastUpdatedDate { get; set; }
    }
}
