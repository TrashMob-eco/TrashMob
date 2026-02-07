#nullable disable

namespace TrashMob.Models
{
    using System;

    public class ProspectActivity : KeyedModel
    {
        public Guid ProspectId { get; set; }

        public string ActivityType { get; set; }

        public string Subject { get; set; }

        public string Details { get; set; }

        public string SentimentScore { get; set; }

        public virtual CommunityProspect Prospect { get; set; }
    }
}
