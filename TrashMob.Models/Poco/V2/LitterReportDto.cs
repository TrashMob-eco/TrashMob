#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// V2 API representation of a litter report with associated images.
    /// </summary>
    public class LitterReportDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the litter report.
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name or title of the litter report.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the litter report.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status identifier (New=1, Assigned=2, Cleaned=3, Cancelled=4).
        /// </summary>
        [JsonPropertyName("litterReportStatusId")]
        public int LitterReportStatusId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the report.
        /// </summary>
        [JsonPropertyName("createdByUserId")]
        public Guid CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the report was created.
        /// </summary>
        [JsonPropertyName("createdDate")]
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets when the report was last updated.
        /// </summary>
        [JsonPropertyName("lastUpdatedDate")]
        public DateTimeOffset LastUpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the associated images for this litter report.
        /// </summary>
        [JsonPropertyName("images")]
        public List<LitterImageDto> Images { get; set; } = [];
    }
}
