namespace TrashMob.Shared
{
    public static class Constants
    {
        public const string TrashMobReadScope = "TrashMob.Read";
        public const string TrashMobWriteScope = "TrashMob.Writes";
        public const string TrashMobEmailAddress = "info@trashmob.eco";
        public const string TrashMobEmailName = "TrashMob Information";
    }

    public enum CommunityContactTypeEnum
    {
        None = 0,
        Official = 1,
        TrashMobHeadquarters = 2,
        TrashMobVolunteer = 3,
        Partner = 4,
    }

    public enum CommunityStatusEnum
    {
        None = 0,
        Submitted = 1,
        ReviewInProgress = 2,
        Approved = 3,
        Declined = 4,
        OutOfDate = 5,
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

    public enum EventPartnerStatusEnum
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

    public enum PartnerRequestStatusEnum
    {
        Pending = 1,
        Approved = 2,
        Denied = 3,
    }

    public enum PartnerStatusEnum
    {
        Active = 1,
        Inactive = 2,
    }

}
