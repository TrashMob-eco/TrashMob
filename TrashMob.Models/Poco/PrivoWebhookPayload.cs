namespace TrashMob.Models.Poco
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Webhook payload sent by PRIVO for consent and account events (Section 10).
    /// </summary>
    public class PrivoWebhookPayload
    {
        /// <summary>
        /// Gets or sets the unique identifier for this webhook event.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp when the event occurred.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the principal SiD (child or adult user).
        /// </summary>
        [JsonPropertyName("sid")]
        public string Sid { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the event types (e.g., "consent_request_created").
        /// </summary>
        [JsonPropertyName("event_types")]
        public List<string> EventTypes { get; set; } = [];

        /// <summary>
        /// Gets or sets the granter (parent) SiDs.
        /// </summary>
        [JsonPropertyName("granter_sid")]
        public List<string> GranterSid { get; set; } = [];

        /// <summary>
        /// Gets or sets the consent identifiers.
        /// </summary>
        [JsonPropertyName("consent_identifiers")]
        public List<string> ConsentIdentifiers { get; set; } = [];
    }
}
