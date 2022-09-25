#nullable disable

namespace TrashMob.Shared.Models
{
    using System;
    using System.Collections.Generic;

    public partial class User : KeyedModel
    {
        public User()
        {
            EventsCreated = new HashSet<Event>();
            EventsUpdated = new HashSet<Event>();
            UserNotifications = new HashSet<UserNotification>();
            NonEventUserNotifications = new HashSet<NonEventUserNotification>();
            PartnersCreated = new HashSet<Partner>();
            PartnersUpdated = new HashSet<Partner>();
        }

        public string NameIdentifier { get; set; }

        public string SourceSystemUserName { get; set; }

        public string UserName { get; set; }

        public string GivenName { get; set; }
        
        public string SurName { get; set; }

        public string Email { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool PrefersMetric { get; set; }

        public int TravelLimitForLocalEvents { get; set; }

        public bool IsSiteAdmin { get; set; }

        public DateTimeOffset? DateAgreedToPrivacyPolicy { get; set; }

        public string PrivacyPolicyVersion { get; set; }

        public DateTimeOffset? DateAgreedToTermsOfService { get; set; }

        public string TermsOfServiceVersion { get; set; }

        public DateTimeOffset? DateAgreedToTrashMobWaiver { get; set; }

        public string TrashMobWaiverVersion { get; set; }

        public DateTimeOffset? MemberSince { get; set; }

        public virtual ICollection<Event> EventsCreated { get; set; }

        public virtual ICollection<Event> EventsUpdated { get; set; }

        public virtual ICollection<UserNotification> UserNotifications { get; set; }
        
        public virtual ICollection<NonEventUserNotification> NonEventUserNotifications { get; set; }

        public virtual ICollection<PartnerRequest> PartnerRequestsCreated { get; set; }

        public virtual ICollection<PartnerRequest> PartnerRequestsUpdated { get; set; }

        public virtual ICollection<Partner> PartnersCreated { get; set; }

        public virtual ICollection<Partner> PartnersUpdated { get; set; }

        public virtual ICollection<PartnerUser> PartnerUsersCreated { get; set; }

        public virtual ICollection<PartnerUser> PartnerUsersUpdated { get; set; }

        public virtual ICollection<PartnerLocation> PartnerLocationsCreated { get; set; }

        public virtual ICollection<PartnerLocation> PartnerLocationsUpdated { get; set; }

        public virtual ICollection<EventPartner> EventPartnersCreated { get; set; }

        public virtual ICollection<EventPartner> EventPartnersUpdated { get; set; }

        public virtual ICollection<EventSummary> EventSummariesCreated { get; set; }

        public virtual ICollection<EventSummary> EventSummariesUpdated { get; set; }

        public virtual ICollection<PartnerDocument> PartnerDocumentsCreated { get; set; }

        public virtual ICollection<PartnerDocument> PartnerDocumentsUpdated { get; set; }

        public virtual ICollection<PartnerContact> PartnerContactsCreated { get; set; }

        public virtual ICollection<PartnerContact> PartnerContactsUpdated { get; set; }

        public virtual ICollection<PartnerSocialMediaAccount> PartnerSocialMediaAccountsCreated { get; set; }

        public virtual ICollection<PartnerSocialMediaAccount> PartnerSocialMediaAccountsUpdated { get; set; }

        public virtual ICollection<NonEventUserNotification> NonEventUserNotificationsCreated { get; set; }

        public virtual ICollection<NonEventUserNotification> NonEventUserNotificationsUpdated { get; set; }

        public virtual ICollection<UserNotification> UserNotificationsCreated { get; set; }

        public virtual ICollection<UserNotification> UserNotificationsUpdated { get; set; }

        public virtual ICollection<User> UsersCreated { get; set; }

        public virtual ICollection<User> UsersUpdated { get; set; }

        public virtual ICollection<ContactRequest> ContactRequestsCreated { get; set; }

        public virtual ICollection<ContactRequest> ContactRequestsUpdated { get; set; }

        public virtual ICollection<MessageRequest> MessageRequestsCreated { get; set; }

        public virtual ICollection<MessageRequest> MessageRequestsUpdated { get; set; }

        public virtual ICollection<SiteMetric> SiteMetricsCreated { get; set; }

        public virtual ICollection<SiteMetric> SiteMetricsUpdated { get; set; }

        public virtual ICollection<PartnerService> PartnerServicesCreated { get; set; }

        public virtual ICollection<PartnerService> PartnerServicesUpdated { get; set; }

        public virtual ICollection<PartnerLocationService> PartnerLocationServicesCreated { get; set; }

        public virtual ICollection<PartnerLocationService> PartnerLocationServicesUpdated { get; set; }
    }
}
