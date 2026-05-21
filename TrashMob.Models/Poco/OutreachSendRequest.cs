namespace TrashMob.Models.Poco
{
    using System;

    /// <summary>
    /// Optional request body for sending an outreach email with user-edited content.
    /// When subject and htmlBody are provided, AI generation is skipped.
    /// </summary>
    public class OutreachSendRequest
    {
        /// <summary>Gets or sets the custom email subject. If null, AI-generated content is used.</summary>
        public string? Subject { get; set; }

        /// <summary>Gets or sets the custom HTML body. If null, AI-generated content is used.</summary>
        public string? HtmlBody { get; set; }

        /// <summary>
        /// Gets or sets the specific ProspectContact to send to. If null, the prospect's
        /// primary active contact is used. Project 60 Phase 2.
        /// </summary>
        public Guid? ProspectContactId { get; set; }
    }
}
