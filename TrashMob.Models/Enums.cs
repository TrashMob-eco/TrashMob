namespace TrashMob.Models
{
    public enum EventPartnerLocationServiceStatusEnum
    {
        None = 0,
        Requested = 1,
        Accepted = 2,
        Declined = 3,
    }

    public enum EventStatusEnum
    {
        Active = 1,
        Full = 2,
        Canceled = 3,
        Complete = 4,
    }

    public enum InvitationStatusEnum
    {
        New = 1,
        Sent = 2,
        Accepted = 3,
        Canceled = 4,
        Declined = 5,
    }

    public enum PartnerRequestStatusEnum
    {
        Sent = 1,
        Approved = 2,
        Denied = 3,
        Pending = 4,
    }

    public enum PartnerStatusEnum
    {
        Active = 1,
        Inactive = 2,
    }

    public enum PartnerTypeEnum
    {
        Government = 1,
        Business = 2,
    }

    public enum ServiceTypeEnum
    {
        Hauling = 1,
        DisposalLocation = 2,
        StartupKits = 3,
        Supplies = 4,
    }

    public enum SocialMediaAccountTypeEnum
    {
        Facebook = 1,
        Twitter = 2,
        Instagram = 3,
        TikTok = 4,
    }

    public enum ImageTypeEnum
    {
        Before = 1,
        After = 2,
        Pickup = 3,
        LitterImage = 4,
    }

    public enum ImageSizeEnum
    {
        Raw = 1,
        Thumb = 2,
        Reduced = 3,
    }

    public enum LitterReportStatusEnum
    {
        New = 1,
        Assigned = 2,
        Cleaned = 3,
        Cancelled = 4,
    }
}