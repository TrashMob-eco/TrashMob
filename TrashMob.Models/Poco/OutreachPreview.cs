namespace TrashMob.Models.Poco
{
    using System;

    /// <summary>
    /// Preview of an AI-generated outreach email before sending.
    /// </summary>
    public class OutreachPreview
    {
        /// <summary>Gets or sets the prospect identifier.</summary>
        public Guid ProspectId { get; set; }

        /// <summary>Gets or sets the prospect name.</summary>
        public string ProspectName { get; set; } = string.Empty;

        /// <summary>Gets or sets the cadence step (1-4).</summary>
        public int CadenceStep { get; set; }

        /// <summary>Gets or sets the generated email subject line.</summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>Gets or sets the generated HTML body.</summary>
        public string HtmlBody { get; set; } = string.Empty;

        /// <summary>Gets or sets the number of AI tokens used for generation.</summary>
        public int TokensUsed { get; set; }
    }
}
