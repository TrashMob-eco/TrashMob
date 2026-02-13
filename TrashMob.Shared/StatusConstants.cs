namespace TrashMob.Shared
{
    public static class NewsletterStatus
    {
        public const string Draft = "Draft";
        public const string Scheduled = "Scheduled";
        public const string Sending = "Sending";
        public const string Sent = "Sent";
        public const string Failed = "Failed";
    }

    public static class NewsletterTargetType
    {
        public const string All = "All";
        public const string Team = "Team";
        public const string Community = "Community";
    }

    public static class EmailInviteStatus
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Sent = "Sent";
        public const string Failed = "Failed";
        public const string Complete = "Complete";
    }

    public static class TeamAdoptionStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }

    public static class EventAttendeeMetricsStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Adjusted = "Adjusted";
    }

    public static class SponsoredAdoptionStatus
    {
        public const string Active = "Active";
        public const string Expired = "Expired";
    }

    public static class ProspectOutreachStatus
    {
        public const string Sent = "Sent";
        public const string Failed = "Failed";
    }

    public static class PhotoModerationAction
    {
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Flagged = "Flagged";
    }
}
