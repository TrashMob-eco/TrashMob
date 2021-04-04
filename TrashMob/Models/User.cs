#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    public partial class User
    {
        public User()
        {
            EventsCreated = new HashSet<Event>();
            EventsUpdated = new HashSet<Event>();
        }

        public Guid Id { get; set; }

        public DateTimeOffset? DateAgreedToPrivacyPolicy { get; set; }

        public string PrivacyPolicyVersion { get; set; }

        public DateTimeOffset? DateAgreedToTermsOfService { get; set; }

        public string TermsOfServiceVersion { get; set; }

        public DateTimeOffset? MemberSince { get; set; }

        public virtual ICollection<Event> EventsCreated { get; set; }

        public virtual ICollection<Event> EventsUpdated { get; set; }
    }
}
