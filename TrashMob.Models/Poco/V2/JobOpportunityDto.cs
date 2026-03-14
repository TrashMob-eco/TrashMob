#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 DTO for job opportunity data.
    /// </summary>
    public class JobOpportunityDto
    {
        /// <summary>Gets or sets the job opportunity ID.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the title.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the tagline.</summary>
        public string TagLine { get; set; } = string.Empty;

        /// <summary>Gets or sets the full description.</summary>
        public string FullDescription { get; set; } = string.Empty;

        /// <summary>Gets or sets whether the opportunity is active.</summary>
        public bool IsActive { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>Gets or sets the last updated date.</summary>
        public DateTimeOffset LastUpdatedDate { get; set; }
    }
}
