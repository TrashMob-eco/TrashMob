#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a community prospect. Flat DTO excluding navigation properties.
    /// </summary>
    public class CommunityProspectDto
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the prospect name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the prospect type.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the region (state/province).
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the population.
        /// </summary>
        public int? Population { get; set; }

        /// <summary>
        /// Gets or sets the website URL.
        /// </summary>
        public string? Website { get; set; }

        /// <summary>
        /// Gets or sets the contact email.
        /// </summary>
        public string? ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the contact name.
        /// </summary>
        public string? ContactName { get; set; }

        /// <summary>
        /// Gets or sets the contact title.
        /// </summary>
        public string? ContactTitle { get; set; }

        /// <summary>
        /// Gets or sets the contact phone.
        /// </summary>
        public string? ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets the pipeline stage.
        /// </summary>
        public int PipelineStage { get; set; }

        /// <summary>
        /// Gets or sets the fit score.
        /// </summary>
        public int FitScore { get; set; }

        /// <summary>
        /// Gets or sets notes about the prospect.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the date of last contact.
        /// </summary>
        public DateTimeOffset? LastContactedDate { get; set; }

        /// <summary>
        /// Gets or sets the next follow-up date.
        /// </summary>
        public DateTimeOffset? NextFollowUpDate { get; set; }

        /// <summary>
        /// Gets or sets the partner identifier if this prospect was converted.
        /// </summary>
        public Guid? ConvertedPartnerId { get; set; }

        /// <summary>
        /// Gets or sets when the prospect was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
