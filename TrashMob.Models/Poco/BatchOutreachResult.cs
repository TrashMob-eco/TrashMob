namespace TrashMob.Models.Poco
{
    using System.Collections.Generic;

    /// <summary>
    /// Result of a batch outreach send operation.
    /// </summary>
    public class BatchOutreachResult
    {
        /// <summary>Gets or sets the total number of prospects requested.</summary>
        public int TotalRequested { get; set; }

        /// <summary>Gets or sets the number of emails successfully sent.</summary>
        public int Sent { get; set; }

        /// <summary>Gets or sets the number of emails that failed to send.</summary>
        public int Failed { get; set; }

        /// <summary>Gets or sets the number of prospects skipped (no email, wrong stage, etc.).</summary>
        public int Skipped { get; set; }

        /// <summary>Gets or sets the per-prospect send results.</summary>
        public List<OutreachSendResult> Results { get; set; } = [];
    }
}
