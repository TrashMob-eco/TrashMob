#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a prospect activity. Flat DTO excluding navigation properties.
    /// </summary>
    public class ProspectActivityDto
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the prospect identifier.
        /// </summary>
        public Guid ProspectId { get; set; }

        /// <summary>
        /// Gets or sets the optional ProspectContact this activity was directed at.
        /// Project 60 Phase 2: lets activities be attributed to a specific person at the prospect.
        /// </summary>
        public Guid? ProspectContactId { get; set; }

        /// <summary>
        /// Gets or sets the activity type.
        /// </summary>
        public string ActivityType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Gets or sets the sentiment score.
        /// </summary>
        public string? SentimentScore { get; set; }

        /// <summary>
        /// Gets or sets when the activity was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
