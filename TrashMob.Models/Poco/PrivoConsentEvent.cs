namespace TrashMob.Models.Poco
{
    using System;

    /// <summary>
    /// Placeholder model for PRIVO consent webhook payloads.
    /// Will be updated when the official PRIVO payload specification is available.
    /// </summary>
    public class PrivoConsentEvent
    {
        /// <summary>
        /// Gets or sets the type of consent status update.
        /// </summary>
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identifier for the consent request.
        /// </summary>
        public string ConsentRequestId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the consent status (e.g., approved, denied).
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets when the event occurred.
        /// </summary>
        public DateTimeOffset? Timestamp { get; set; }
    }
}
