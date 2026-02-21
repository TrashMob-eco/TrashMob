#nullable disable

namespace TrashMob.Models
{
    using System;

    /// <summary>
    /// Represents an outreach email sent to a community prospect as part of the cadence engine.
    /// </summary>
    public class ProspectOutreachEmail : KeyedModel
    {
        /// <summary>
        /// Gets or sets the prospect this email was sent to.
        /// </summary>
        public Guid ProspectId { get; set; }

        /// <summary>
        /// Gets or sets the cadence step (1=Initial, 2=Follow-up, 3=Value-add, 4=Final).
        /// </summary>
        public int CadenceStep { get; set; }

        /// <summary>
        /// Gets or sets the email subject line.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the HTML body of the email.
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// Gets or sets the email status (Draft, Sent, Delivered, Opened, Clicked, Bounced, Failed).
        /// </summary>
        public string Status { get; set; } = "Draft";

        /// <summary>
        /// Gets or sets when the email was sent.
        /// </summary>
        public DateTimeOffset? SentDate { get; set; }

        /// <summary>
        /// Gets or sets when the email was opened.
        /// </summary>
        public DateTimeOffset? OpenedDate { get; set; }

        /// <summary>
        /// Gets or sets when a link in the email was clicked.
        /// </summary>
        public DateTimeOffset? ClickedDate { get; set; }

        /// <summary>
        /// Gets or sets an error message if the send failed.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the associated prospect.
        /// </summary>
        public virtual CommunityProspect Prospect { get; set; }
    }
}
