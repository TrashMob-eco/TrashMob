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

    public enum ImageTypeEnum
    {
        Before = 1,
        After = 2,
        Pickup = 3,
    }
}
