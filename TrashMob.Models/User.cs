#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a user in the TrashMob system.
    /// </summary>
    public class User : KeyedModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
            EventsCreated = new HashSet<Event>();
            EventsUpdated = new HashSet<Event>();
            UserNotifications = new HashSet<UserNotification>();
            NonEventUserNotifications = new HashSet<NonEventUserNotification>();
            JobOpportunitiesCreated = new HashSet<JobOpportunity>();
            JobOpportunitiesUpdated = new HashSet<JobOpportunity>();
            PartnersUpdated = new HashSet<Partner>();
            PartnersCreated = new HashSet<Partner>();
            PartnersUpdated = new HashSet<Partner>();
            EventAttendees = new HashSet<EventAttendee>();
            EventAttendeeRoutes = new HashSet<EventAttendeeRoute>();
        }

        /// <summary>
        /// Gets or sets the object identifier from the identity provider.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the name identifier claim from the identity provider.
        /// </summary>
        public string NameIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the username from the source identity system.
        /// </summary>
        public string SourceSystemUserName { get; set; }

        /// <summary>
        /// Gets or sets the display username for the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the city where the user is located.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the region or state where the user is located.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the country where the user is located.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the user's location.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the latitude coordinate of the user's location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate of the user's location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user prefers metric units.
        /// </summary>
        public bool PrefersMetric { get; set; }

        /// <summary>
        /// Gets or sets the maximum travel distance (in miles or kilometers) the user is willing to travel for local events.
        /// </summary>
        public int TravelLimitForLocalEvents { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is a site administrator.
        /// </summary>
        public bool IsSiteAdmin { get; set; }

        /// <summary>
        /// Gets or sets the date when the user agreed to the TrashMob waiver.
        /// </summary>
        public DateTimeOffset? DateAgreedToTrashMobWaiver { get; set; }

        /// <summary>
        /// Gets or sets the version of the TrashMob waiver the user agreed to.
        /// </summary>
        public string TrashMobWaiverVersion { get; set; }

        /// <summary>
        /// Gets or sets the date when the user became a member.
        /// </summary>
        public DateTimeOffset? MemberSince { get; set; }

        /// <summary>
        /// Gets or sets the collection of events the user is attending.
        /// </summary>
        public virtual ICollection<EventAttendee> EventAttendees { get; set; }

        /// <summary>
        /// Gets or sets the collection of routes the user has taken during events.
        /// </summary>
        public virtual ICollection<EventAttendeeRoute> EventAttendeeRoutes { get; set; }

        /// <summary>
        /// Gets or sets the collection of events created by this user.
        /// </summary>
        public virtual ICollection<Event> EventsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of events last updated by this user.
        /// </summary>
        public virtual ICollection<Event> EventsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of notifications for this user.
        /// </summary>
        public virtual ICollection<UserNotification> UserNotifications { get; set; }

        /// <summary>
        /// Gets or sets the collection of non-event notifications for this user.
        /// </summary>
        public virtual ICollection<NonEventUserNotification> NonEventUserNotifications { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner requests created by this user.
        /// </summary>
        public virtual ICollection<PartnerRequest> PartnerRequestsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner requests last updated by this user.
        /// </summary>
        public virtual ICollection<PartnerRequest> PartnerRequestsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partners created by this user.
        /// </summary>
        public virtual ICollection<Partner> PartnersCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partners last updated by this user.
        /// </summary>
        public virtual ICollection<Partner> PartnersUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner admins created by this user.
        /// </summary>
        public virtual ICollection<PartnerAdmin> PartnerAdminsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner admins last updated by this user.
        /// </summary>
        public virtual ICollection<PartnerAdmin> PartnerAdminsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner admin invitations created by this user.
        /// </summary>
        public virtual ICollection<PartnerAdminInvitation> PartnerAdminInvitationsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner admin invitations last updated by this user.
        /// </summary>
        public virtual ICollection<PartnerAdminInvitation> PartnerAdminInvitationsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner locations created by this user.
        /// </summary>
        public virtual ICollection<PartnerLocation> PartnerLocationsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner locations last updated by this user.
        /// </summary>
        public virtual ICollection<PartnerLocation> PartnerLocationsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of event partner locations created by this user.
        /// </summary>
        public virtual ICollection<EventPartnerLocationService> EventPartnerLocationsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of event partner locations last updated by this user.
        /// </summary>
        public virtual ICollection<EventPartnerLocationService> EventPartnerLocationsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of event summaries created by this user.
        /// </summary>
        public virtual ICollection<EventSummary> EventSummariesCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of event summaries last updated by this user.
        /// </summary>
        public virtual ICollection<EventSummary> EventSummariesUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of IFTTT triggers created by this user.
        /// </summary>
        public virtual ICollection<IftttTrigger> IftttTriggersCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of IFTTT triggers last updated by this user.
        /// </summary>
        public virtual ICollection<IftttTrigger> IftttTriggersUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of job opportunities created by this user.
        /// </summary>
        public virtual ICollection<JobOpportunity> JobOpportunitiesCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of job opportunities last updated by this user.
        /// </summary>
        public virtual ICollection<JobOpportunity> JobOpportunitiesUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner documents created by this user.
        /// </summary>
        public virtual ICollection<PartnerDocument> PartnerDocumentsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner documents last updated by this user.
        /// </summary>
        public virtual ICollection<PartnerDocument> PartnerDocumentsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner contacts created by this user.
        /// </summary>
        public virtual ICollection<PartnerContact> PartnerContactsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner contacts last updated by this user.
        /// </summary>
        public virtual ICollection<PartnerContact> PartnerContactsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner social media accounts created by this user.
        /// </summary>
        public virtual ICollection<PartnerSocialMediaAccount> PartnerSocialMediaAccountsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner social media accounts last updated by this user.
        /// </summary>
        public virtual ICollection<PartnerSocialMediaAccount> PartnerSocialMediaAccountsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of non-event user notifications created by this user.
        /// </summary>
        public virtual ICollection<NonEventUserNotification> NonEventUserNotificationsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of non-event user notifications last updated by this user.
        /// </summary>
        public virtual ICollection<NonEventUserNotification> NonEventUserNotificationsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of user notifications created by this user.
        /// </summary>
        public virtual ICollection<UserNotification> UserNotificationsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of user notifications last updated by this user.
        /// </summary>
        public virtual ICollection<UserNotification> UserNotificationsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of users created by this user.
        /// </summary>
        public virtual ICollection<User> UsersCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of users last updated by this user.
        /// </summary>
        public virtual ICollection<User> UsersUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of contact requests created by this user.
        /// </summary>
        public virtual ICollection<ContactRequest> ContactRequestsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of contact requests last updated by this user.
        /// </summary>
        public virtual ICollection<ContactRequest> ContactRequestsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of message requests created by this user.
        /// </summary>
        public virtual ICollection<MessageRequest> MessageRequestsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of message requests last updated by this user.
        /// </summary>
        public virtual ICollection<MessageRequest> MessageRequestsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of site metrics created by this user.
        /// </summary>
        public virtual ICollection<SiteMetric> SiteMetricsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of site metrics last updated by this user.
        /// </summary>
        public virtual ICollection<SiteMetric> SiteMetricsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner location contacts created by this user.
        /// </summary>
        public virtual ICollection<PartnerLocationContact> PartnerLocationContactsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner location contacts last updated by this user.
        /// </summary>
        public virtual ICollection<PartnerLocationContact> PartnerLocationContactsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner location services created by this user.
        /// </summary>
        public virtual ICollection<PartnerLocationService> PartnerLocationServicesCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner location services last updated by this user.
        /// </summary>
        public virtual ICollection<PartnerLocationService> PartnerLocationServicesUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of event attendees created by this user.
        /// </summary>
        public virtual ICollection<EventAttendee> EventAttendeesCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of event attendees last updated by this user.
        /// </summary>
        public virtual ICollection<EventAttendee> EventAttendeesUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of event attendee routes created by this user.
        /// </summary>
        public virtual ICollection<EventAttendeeRoute> EventAttendeeRoutesCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of event attendee routes last updated by this user.
        /// </summary>
        public virtual ICollection<EventAttendeeRoute> EventAttendeeRoutesUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of waivers created by this user.
        /// </summary>
        public virtual ICollection<Waiver> WaiverStatusesCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of waivers last updated by this user.
        /// </summary>
        public virtual ICollection<Waiver> WaiverStatusesUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of pickup locations created by this user.
        /// </summary>
        public virtual ICollection<PickupLocation> PickupLocationsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of pickup locations last updated by this user.
        /// </summary>
        public virtual ICollection<PickupLocation> PickupLocationsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of event litter reports created by this user.
        /// </summary>
        public virtual ICollection<EventLitterReport> EventLitterReportsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of event litter reports last updated by this user.
        /// </summary>
        public virtual ICollection<EventLitterReport> EventLitterReportsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of litter reports created by this user.
        /// </summary>
        public virtual ICollection<LitterReport> LitterReportsCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of litter reports last updated by this user.
        /// </summary>
        public virtual ICollection<LitterReport> LitterReportsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the collection of litter images created by this user.
        /// </summary>
        public virtual ICollection<LitterImage> LitterImagesCreated { get; set; }

        /// <summary>
        /// Gets or sets the collection of litter images last updated by this user.
        /// </summary>
        public virtual ICollection<LitterImage> LitterImagesUpdated { get; set; }
    }
}