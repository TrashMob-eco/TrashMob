#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a staged adoptable area awaiting review. Flat DTO excluding navigation properties.
    /// </summary>
    public class StagedAdoptableAreaDto
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the batch that produced this staged area.
        /// </summary>
        public Guid BatchId { get; set; }

        /// <summary>
        /// Gets or sets the community (partner) this staged area belongs to.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the area name (from OSM or AI-generated).
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the area description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the type of area (School, Park, etc.).
        /// </summary>
        public string? AreaType { get; set; }

        /// <summary>
        /// Gets or sets the GeoJSON representation of the area boundary.
        /// </summary>
        public string? GeoJson { get; set; }

        /// <summary>
        /// Gets or sets the center latitude of the area.
        /// </summary>
        public double CenterLatitude { get; set; }

        /// <summary>
        /// Gets or sets the center longitude of the area.
        /// </summary>
        public double CenterLongitude { get; set; }

        /// <summary>
        /// Gets or sets the review status (Pending, Approved, Rejected).
        /// </summary>
        public string ReviewStatus { get; set; } = "Pending";

        /// <summary>
        /// Gets or sets the confidence level (High, Medium, Low).
        /// </summary>
        public string Confidence { get; set; } = "Medium";

        /// <summary>
        /// Gets or sets whether this area is a potential duplicate of an existing adoptable area.
        /// </summary>
        public bool IsPotentialDuplicate { get; set; }

        /// <summary>
        /// Gets or sets the name of the existing area this may duplicate.
        /// </summary>
        public string? DuplicateOfName { get; set; }

        /// <summary>
        /// Gets or sets the OpenStreetMap identifier for traceability.
        /// </summary>
        public string? OsmId { get; set; }

        /// <summary>
        /// Gets or sets a JSON representation of relevant OSM tags.
        /// </summary>
        public string? OsmTags { get; set; }

        /// <summary>
        /// Gets or sets when the staged area was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
