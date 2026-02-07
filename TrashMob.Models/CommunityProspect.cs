#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    public class CommunityProspect : KeyedModel
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int? Population { get; set; }

        public string Website { get; set; }

        public string ContactEmail { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        public int PipelineStage { get; set; }

        public int FitScore { get; set; }

        public string Notes { get; set; }

        public DateTimeOffset? LastContactedDate { get; set; }

        public DateTimeOffset? NextFollowUpDate { get; set; }

        public Guid? ConvertedPartnerId { get; set; }

        public virtual ICollection<ProspectActivity> Activities { get; set; } = [];

        public virtual ICollection<ProspectOutreachEmail> OutreachEmails { get; set; } = [];
    }
}
