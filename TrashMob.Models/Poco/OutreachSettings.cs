namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Configuration settings for the prospect outreach system.
    /// Read from IConfiguration (environment variables / Key Vault).
    /// </summary>
    public class OutreachSettings
    {
        /// <summary>Gets or sets whether outreach sending is enabled. Default false.</summary>
        public bool OutreachEnabled { get; set; }

        /// <summary>Gets or sets whether test mode is active. When true, emails go to TestRecipientEmail instead of prospects. Default true.</summary>
        public bool TestMode { get; set; } = true;

        /// <summary>Gets or sets the email address that receives test emails in test mode.</summary>
        public string TestRecipientEmail { get; set; } = "info@trashmob.eco";

        /// <summary>Gets or sets the maximum number of outreach emails to send per day.</summary>
        public int MaxDailyOutreach { get; set; } = 10;

        /// <summary>Gets or sets the maximum number of follow-up emails per hourly job run.</summary>
        public int MaxFollowUpsPerRun { get; set; } = 10;
    }
}
