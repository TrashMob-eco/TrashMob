#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;
    using System.Collections.Generic;

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
        /// Gets or sets the constrained municipality type (see
        /// <see cref="MunicipalityTypeEnum"/>). Wire format is the enum name
        /// (e.g. <c>"City"</c>, <c>"County"</c>) so the client does not depend
        /// on integer ordering.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the original free-form <c>Type</c> string captured
        /// before the Project 63 <see cref="MunicipalityTypeEnum"/> migration.
        /// Read-only from the UI's perspective — retained for audit.
        /// </summary>
        public string? TypeRaw { get; set; }

        /// <summary>
        /// Gets or sets the department within the municipality being worked
        /// (e.g. Public Works, Sustainability). Aggregated on the weekly
        /// report's "best responding departments" section.
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Gets or sets the sales priority (see <see cref="ProspectPriorityEnum"/>).
        /// Nullable — a newly-discovered prospect has no priority yet.
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// Gets or sets free-text pricing / business-model feedback captured
        /// from the prospect. Aggregated on the weekly report.
        /// </summary>
        public string? PricingFeedback { get; set; }

        /// <summary>
        /// Gets or sets the free-text key objection or open question from the
        /// prospect. Aggregated on the weekly report.
        /// </summary>
        public string? KeyObjection { get; set; }

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
        /// Gets or sets the primary contact email. Backward-compat: maps to the prospect's
        /// primary <see cref="ProspectContactDto"/>. Will be removed once frontend migrates
        /// to the dedicated /contacts endpoints (Project 60 Phase 3).
        /// </summary>
        public string? ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the primary contact name. Backward-compat shortcut for the primary
        /// <see cref="ProspectContactDto"/>.
        /// </summary>
        public string? ContactName { get; set; }

        /// <summary>
        /// Gets or sets the primary contact title. Backward-compat shortcut for the primary
        /// <see cref="ProspectContactDto"/>.
        /// </summary>
        public string? ContactTitle { get; set; }

        /// <summary>
        /// Gets or sets the primary contact phone. Backward-compat shortcut for the primary
        /// <see cref="ProspectContactDto"/>.
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
        /// Gets or sets the contacts associated with this prospect (Project 60).
        /// Populated on read; ignored on write — contacts are managed via the dedicated
        /// /contacts endpoints introduced in Phase 2.
        /// </summary>
        public List<ProspectContactDto> Contacts { get; set; } = [];

        /// <summary>
        /// Gets or sets when the prospect was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
