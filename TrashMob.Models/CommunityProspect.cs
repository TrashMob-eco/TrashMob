#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    public class CommunityProspect : KeyedModel
    {
        public string Name { get; set; }

        /// <summary>
        /// Constrained municipality type (see <see cref="MunicipalityTypeEnum"/>).
        /// Populated by the salesperson in the admin UI. During the Project 63
        /// migration this is derived from the legacy free-form value preserved
        /// in <see cref="TypeRaw"/>.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Original free-form <c>Type</c> string as captured before the
        /// Project 63 <see cref="MunicipalityTypeEnum"/> migration. Retained
        /// for audit; the salesperson works with <see cref="Type"/> going forward.
        /// </summary>
        public string TypeRaw { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int? Population { get; set; }

        public string Website { get; set; }

        /// <summary>
        /// Department within the municipality that we're targeting or already
        /// speaking with (e.g. Public Works, Sustainability, Parks). Free-form
        /// on purpose — spelling variations are aggregated in the weekly
        /// "best responding departments" report (Project 63).
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Sales priority ranking (see <see cref="ProspectPriorityEnum"/>).
        /// Nullable — a newly discovered prospect has no priority yet.
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// Free-text feedback from the prospect on pricing / business model
        /// (e.g. "outside our current budget cycle", "prefer usage-based").
        /// Aggregated on the weekly report.
        /// </summary>
        public string PricingFeedback { get; set; }

        /// <summary>
        /// Free-text objection or open question captured from the prospect
        /// (e.g. "concerned about volunteer waivers"). Aggregated on the
        /// weekly report.
        /// </summary>
        public string KeyObjection { get; set; }

        /// <summary>
        /// Ordered pipeline stage (see <see cref="PipelineStageEnum"/>). Stored
        /// as an int for backwards compatibility with the pre-Project-63 wire
        /// format.
        /// </summary>
        public int PipelineStage { get; set; }

        public int FitScore { get; set; }

        public string Notes { get; set; }

        public DateTimeOffset? LastContactedDate { get; set; }

        public DateTimeOffset? NextFollowUpDate { get; set; }

        public Guid? ConvertedPartnerId { get; set; }

        public virtual ICollection<ProspectContact> Contacts { get; set; } = [];

        public virtual ICollection<ProspectActivity> Activities { get; set; } = [];

        public virtual ICollection<ProspectOutreachEmail> OutreachEmails { get; set; } = [];
    }
}
