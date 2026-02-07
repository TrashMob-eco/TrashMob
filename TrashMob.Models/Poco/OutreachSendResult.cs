namespace TrashMob.Models.Poco
{
    using System;

    /// <summary>
    /// Result of sending a single outreach email.
    /// </summary>
    public class OutreachSendResult
    {
        /// <summary>Gets or sets the outreach email record identifier.</summary>
        public Guid ProspectOutreachEmailId { get; set; }

        /// <summary>Gets or sets whether the send was successful.</summary>
        public bool Success { get; set; }

        /// <summary>Gets or sets an error message if the send failed.</summary>
        public string? ErrorMessage { get; set; }
    }
}
