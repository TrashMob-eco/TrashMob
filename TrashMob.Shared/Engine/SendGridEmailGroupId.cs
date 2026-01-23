namespace TrashMob.Shared.Engine
{
    /// <summary>
    /// Contains SendGrid unsubscribe group IDs for different email categories.
    /// </summary>
    public static class SendGridEmailGroupId
    {
        /// <summary>
        /// Unsubscribe group ID for event-related emails (reminders, updates, etc.).
        /// </summary>
        public const int EventRelated = 24774;

        /// <summary>
        /// Unsubscribe group ID for litter report-related emails.
        /// </summary>
        public const int LitterReportRelated = 30841;

        /// <summary>
        /// Unsubscribe group ID for general TrashMob communications.
        /// </summary>
        public const int General = 24775;
    }
}