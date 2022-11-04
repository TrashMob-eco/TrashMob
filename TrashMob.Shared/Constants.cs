namespace TrashMob.Shared
{
    public static class Constants
    {
        public const string TrashMobReadScope = "TrashMob.Read";
        public const string TrashMobWriteScope = "TrashMob.Writes";
        public const string TrashMobEmailAddress = "info@trashmob.eco";
        public const string TrashMobEmailName = "TrashMob Information";
    }

    public enum CommunityStatusEnum
    {
        Inactive = 0,
        Active = 1,
    }

    public enum CommunityTypeEnum
    {
        Other = 0,
        Region = 1,
        County = 2,
        Municipality = 3,
        Neighborhood = 4,
    }

    public enum DisposalTypeEnum
    {
        NotDetermined = 0,
        UseExistingCans = 1,
        Region = 2,
        County = 3,
        Municipality = 4,
        Neighborhood = 5,
        Vendor = 6,
        Volunteer = 7,
        Partner = 8,
        Other = 99,
    }

    public enum HaulingTypeEnum
    {
        NotDetermined = 0,
        Region = 1,
        County = 2,
        Municipality = 3,
        Neighborhood = 4,
        Vendor = 5,
        Volunteer = 6,
        Partner = 7,
        Other = 99,
    }

    public enum StarterKitDeliveryTypeEnum
    {
        NotDetermined = 0,
        Region = 1,
        County = 2,
        Municipality = 3,
        Neighborhood = 4,
        Vendor = 5,
        Volunteer = 6,
        Partner = 7,
        Online = 8,
        Other = 99,
        NotAvailable = 100
    }

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
        Canceled = 4
    }

    public enum PartnerRequestStatusEnum
    {
        Sent = 1,
        Approved = 2,
        Denied = 3,
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
}
