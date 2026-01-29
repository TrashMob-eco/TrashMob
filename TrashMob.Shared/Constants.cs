namespace TrashMob.Shared
{
    /// <summary>
    /// Contains application-wide constant values used throughout TrashMob.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The OAuth scope required for read access to TrashMob resources.
        /// </summary>
        public const string TrashMobReadScope = "TrashMob.Read";

        /// <summary>
        /// The OAuth scope required for write access to TrashMob resources.
        /// </summary>
        public const string TrashMobWriteScope = "TrashMob.Writes";

        /// <summary>
        /// The official TrashMob email address used for sending notifications.
        /// </summary>
        public const string TrashMobEmailAddress = "info@trashmob.eco";

        /// <summary>
        /// The display name used when sending emails from TrashMob.
        /// </summary>
        public const string TrashMobEmailName = "TrashMob Information";

        /// <summary>
        /// The OAuth scope for IFTTT integration.
        /// </summary>
        public const string TrashMobIFTTTScope = "ifttt";
    }
}