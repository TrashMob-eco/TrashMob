#nullable disable

namespace TrashMob.Shared.Models
{
    using System;
    using System.Collections.Generic;

    public partial class User
    {
        public User()
        {
            EventsCreated = new HashSet<Event>();
            EventsUpdated = new HashSet<Event>();
            UserNotifications = new HashSet<UserNotification>();
        }

        public Guid Id { get; set; }

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

        public DateTimeOffset? DateAgreedToPrivacyPolicy { get; set; }

        public string PrivacyPolicyVersion { get; set; }

        public DateTimeOffset? DateAgreedToTermsOfService { get; set; }

        public string TermsOfServiceVersion { get; set; }

        public DateTimeOffset? MemberSince { get; set; }

        public virtual ICollection<Event> EventsCreated { get; set; }

        public virtual ICollection<Event> EventsUpdated { get; set; }

        public virtual ICollection<UserNotification> UserNotifications { get; set; }
    }
}
