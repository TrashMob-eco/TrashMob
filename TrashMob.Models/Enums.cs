namespace TrashMob.Models
{
    /// <summary>
    /// Represents the status of a partner location service request for an event.
    /// </summary>
    public enum EventPartnerLocationServiceStatusEnum
    {
        /// <summary>
        /// No status assigned.
        /// </summary>
        None = 0,

        /// <summary>
        /// Service has been requested.
        /// </summary>
        Requested = 1,

        /// <summary>
        /// Service request has been accepted.
        /// </summary>
        Accepted = 2,

        /// <summary>
        /// Service request has been declined.
        /// </summary>
        Declined = 3,
    }

    /// <summary>
    /// Represents the status of an event.
    /// </summary>
    public enum EventStatusEnum
    {
        /// <summary>
        /// Event is active and accepting participants.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Event has reached maximum capacity.
        /// </summary>
        Full = 2,

        /// <summary>
        /// Event has been canceled.
        /// </summary>
        Canceled = 3,

        /// <summary>
        /// Event has been completed.
        /// </summary>
        Complete = 4,
    }

    /// <summary>
    /// Represents the status of an invitation.
    /// </summary>
    public enum InvitationStatusEnum
    {
        /// <summary>
        /// Invitation has been created but not yet sent.
        /// </summary>
        New = 1,

        /// <summary>
        /// Invitation has been sent to the recipient.
        /// </summary>
        Sent = 2,

        /// <summary>
        /// Invitation has been accepted by the recipient.
        /// </summary>
        Accepted = 3,

        /// <summary>
        /// Invitation has been canceled.
        /// </summary>
        Canceled = 4,

        /// <summary>
        /// Invitation has been declined by the recipient.
        /// </summary>
        Declined = 5,
    }

    /// <summary>
    /// Represents the status of a partner request.
    /// </summary>
    public enum PartnerRequestStatusEnum
    {
        /// <summary>
        /// Request has been sent.
        /// </summary>
        Sent = 1,

        /// <summary>
        /// Request has been approved.
        /// </summary>
        Approved = 2,

        /// <summary>
        /// Request has been denied.
        /// </summary>
        Denied = 3,

        /// <summary>
        /// Request is pending review.
        /// </summary>
        Pending = 4,
    }

    /// <summary>
    /// Represents the status of a partner.
    /// </summary>
    public enum PartnerStatusEnum
    {
        /// <summary>
        /// Partner is active.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Partner is inactive.
        /// </summary>
        Inactive = 2,
    }

    /// <summary>
    /// Represents the type of partner organization.
    /// </summary>
    public enum PartnerTypeEnum
    {
        /// <summary>
        /// Government organization.
        /// </summary>
        Government = 1,

        /// <summary>
        /// Business organization.
        /// </summary>
        Business = 2,

        /// <summary>
        /// Community organization (city, county, nonprofit) with branded community page.
        /// </summary>
        Community = 3,
    }

    /// <summary>
    /// Represents the type of service a partner can provide.
    /// </summary>
    public enum ServiceTypeEnum
    {
        /// <summary>
        /// Hauling service for collected trash.
        /// </summary>
        Hauling = 1,

        /// <summary>
        /// Location for trash disposal.
        /// </summary>
        DisposalLocation = 2,

        /// <summary>
        /// Startup kits for new events.
        /// </summary>
        StartupKits = 3,

        /// <summary>
        /// General supplies for cleanup events.
        /// </summary>
        Supplies = 4,
    }

    /// <summary>
    /// Represents the type of social media platform.
    /// </summary>
    public enum SocialMediaAccountTypeEnum
    {
        /// <summary>
        /// Facebook account.
        /// </summary>
        Facebook = 1,

        /// <summary>
        /// Twitter account.
        /// </summary>
        Twitter = 2,

        /// <summary>
        /// Instagram account.
        /// </summary>
        Instagram = 3,

        /// <summary>
        /// TikTok account.
        /// </summary>
        TikTok = 4,
    }

    /// <summary>
    /// Represents the type of image associated with an event.
    /// </summary>
    public enum ImageTypeEnum
    {
        /// <summary>
        /// Image taken before the cleanup event.
        /// </summary>
        Before = 1,

        /// <summary>
        /// Image taken after the cleanup event.
        /// </summary>
        After = 2,

        /// <summary>
        /// Image of a pickup location.
        /// </summary>
        Pickup = 3,

        /// <summary>
        /// Image associated with a litter report.
        /// </summary>
        LitterImage = 4,

        /// <summary>
        /// Image associated with a team photo gallery.
        /// </summary>
        TeamPhoto = 5,

        /// <summary>
        /// Logo/avatar image for a team.
        /// </summary>
        TeamLogo = 6,

        /// <summary>
        /// Image associated with an event photo gallery.
        /// </summary>
        EventPhoto = 7,

        /// <summary>
        /// Image associated with a partner/community photo gallery.
        /// </summary>
        PartnerPhoto = 8,

        /// <summary>
        /// User profile photo (uploaded or replaced from social IDP).
        /// </summary>
        UserProfilePhoto = 9,

        /// <summary>
        /// Logo image for a community.
        /// </summary>
        CommunityLogo = 10,

        /// <summary>
        /// Banner image for a community.
        /// </summary>
        CommunityBanner = 11,

        /// <summary>
        /// Logo image for a sponsor.
        /// </summary>
        SponsorLogo = 12,
    }

    /// <summary>
    /// Represents the size of an image.
    /// </summary>
    public enum ImageSizeEnum
    {
        /// <summary>
        /// Original raw image size.
        /// </summary>
        Raw = 1,

        /// <summary>
        /// Thumbnail size image.
        /// </summary>
        Thumb = 2,

        /// <summary>
        /// Reduced size image.
        /// </summary>
        Reduced = 3,
    }

    /// <summary>
    /// Represents the status of a litter report.
    /// </summary>
    public enum LitterReportStatusEnum
    {
        /// <summary>
        /// New litter report.
        /// </summary>
        New = 1,

        /// <summary>
        /// Litter report has been assigned to an event.
        /// </summary>
        Assigned = 2,

        /// <summary>
        /// Litter has been cleaned up.
        /// </summary>
        Cleaned = 3,

        /// <summary>
        /// Litter report has been cancelled.
        /// </summary>
        Cancelled = 4,
    }

    /// <summary>
    /// Represents the unit of measurement for weight.
    /// </summary>
    public enum WeightUnitEnum
    {
        /// <summary>
        /// No weight unit specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Weight measured in pounds.
        /// </summary>
        Pound = 1,

        /// <summary>
        /// Weight measured in kilograms.
        /// </summary>
        Kilogram = 2,
    }

    /// <summary>
    /// The type of geographic region a community covers.
    /// </summary>
    public enum RegionTypeEnum
    {
        City = 0,
        County = 1,
        State = 2,
        Province = 3,
        Region = 4,
        Country = 5,
    }

    /// <summary>
    /// Represents the visibility level of an event.
    /// </summary>
    public enum EventVisibilityEnum
    {
        /// <summary>
        /// Event is visible to all users and included in public listings and notifications.
        /// </summary>
        Public = 1,

        /// <summary>
        /// Event is visible only to members of the specified team.
        /// </summary>
        TeamOnly = 2,

        /// <summary>
        /// Event is visible only to the creator, for personal tracking.
        /// </summary>
        Private = 3,
    }

    /// <summary>
    /// Represents the type of a partner document.
    /// </summary>
    public enum PartnerDocumentTypeEnum
    {
        /// <summary>
        /// Other or unspecified document type.
        /// </summary>
        Other = 0,

        /// <summary>
        /// Partnership or volunteer agreement.
        /// </summary>
        Agreement = 1,

        /// <summary>
        /// Formal contract.
        /// </summary>
        Contract = 2,

        /// <summary>
        /// Report or summary document.
        /// </summary>
        Report = 3,

        /// <summary>
        /// Insurance certificate or policy.
        /// </summary>
        Insurance = 4,

        /// <summary>
        /// Certification or compliance document.
        /// </summary>
        Certificate = 5,
    }

    /// <summary>
    /// Represents the type of contact in the CRM system.
    /// </summary>
    public enum ContactTypeEnum
    {
        /// <summary>
        /// An individual person (donor, prospect, volunteer, etc.).
        /// </summary>
        Individual = 1,

        /// <summary>
        /// An organization (company, nonprofit, agency, etc.).
        /// </summary>
        Organization = 2,

        /// <summary>
        /// A foundation or grant-making organization.
        /// </summary>
        Foundation = 3,
    }

    /// <summary>
    /// Represents the type of donation received.
    /// </summary>
    public enum DonationTypeEnum
    {
        /// <summary>
        /// Cash donation.
        /// </summary>
        Cash = 1,

        /// <summary>
        /// Payment by check.
        /// </summary>
        Check = 2,

        /// <summary>
        /// Online payment (credit card, digital wallet, etc.).
        /// </summary>
        Online = 3,

        /// <summary>
        /// In-kind donation of goods or services.
        /// </summary>
        InKind = 4,

        /// <summary>
        /// Employer matching gift.
        /// </summary>
        MatchingGift = 5,
    }

    /// <summary>
    /// Represents the frequency of a recurring donation or pledge.
    /// </summary>
    public enum DonationFrequencyEnum
    {
        /// <summary>
        /// A single one-time gift.
        /// </summary>
        OneTime = 1,

        /// <summary>
        /// Monthly recurring donation.
        /// </summary>
        Monthly = 2,

        /// <summary>
        /// Quarterly recurring donation.
        /// </summary>
        Quarterly = 3,

        /// <summary>
        /// Annual recurring donation.
        /// </summary>
        Annually = 4,
    }

    /// <summary>
    /// Represents the status of a pledge.
    /// </summary>
    public enum PledgeStatusEnum
    {
        /// <summary>
        /// Pledge is actively being fulfilled.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Pledge has been fully fulfilled.
        /// </summary>
        Fulfilled = 2,

        /// <summary>
        /// Pledge payments have lapsed.
        /// </summary>
        Lapsed = 3,

        /// <summary>
        /// Pledge has been cancelled.
        /// </summary>
        Cancelled = 4,
    }

    /// <summary>
    /// Represents the type of a contact interaction note.
    /// </summary>
    public enum ContactNoteTypeEnum
    {
        /// <summary>
        /// Phone call interaction.
        /// </summary>
        Call = 1,

        /// <summary>
        /// In-person or virtual meeting.
        /// </summary>
        Meeting = 2,

        /// <summary>
        /// Email correspondence.
        /// </summary>
        Email = 3,

        /// <summary>
        /// General observation or note.
        /// </summary>
        Note = 4,

        /// <summary>
        /// Thank-you communication.
        /// </summary>
        ThankYou = 5,

        /// <summary>
        /// Fundraising appeal.
        /// </summary>
        Appeal = 6,
    }

    /// <summary>
    /// Represents the status of a grant in the pipeline.
    /// </summary>
    public enum GrantStatusEnum
    {
        /// <summary>
        /// Grant opportunity identified but not yet pursued.
        /// </summary>
        Prospect = 1,

        /// <summary>
        /// Letter of inquiry has been submitted.
        /// </summary>
        LOISubmitted = 2,

        /// <summary>
        /// Full application has been submitted.
        /// </summary>
        ApplicationSubmitted = 3,

        /// <summary>
        /// Grant has been awarded.
        /// </summary>
        Awarded = 4,

        /// <summary>
        /// Grant application was declined.
        /// </summary>
        Declined = 5,

        /// <summary>
        /// Grant is in the reporting phase.
        /// </summary>
        Reporting = 6,

        /// <summary>
        /// Grant is up for renewal.
        /// </summary>
        Renewal = 7,

        /// <summary>
        /// Grant has been closed.
        /// </summary>
        Closed = 8,
    }
}