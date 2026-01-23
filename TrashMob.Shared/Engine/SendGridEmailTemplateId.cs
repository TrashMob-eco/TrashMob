namespace TrashMob.Shared.Engine
{
    /// <summary>
    /// Contains SendGrid dynamic template IDs for different email types.
    /// </summary>
    public static class SendGridEmailTemplateId
    {
        /// <summary>
        /// Template ID for event-related notification emails.
        /// </summary>
        public const string EventEmail = "d-11fedbf069ae4c098a3e837fe45d3fe1";

        /// <summary>
        /// Template ID for generic TrashMob emails.
        /// </summary>
        public const string GenericEmail = "d-a485d1a8e98d4038b2ab34b1daed6196";

        /// <summary>
        /// Template ID for pickup location notification emails.
        /// </summary>
        public const string PickupEmail = "d-50e4ea3024c7459092e96b17c2895dc3";

        /// <summary>
        /// Template ID for litter report notification emails.
        /// </summary>
        public const string LitterReportEmail = "d-f1b8a32c4eef4e1592ece19f0a024c3e";
    }
}