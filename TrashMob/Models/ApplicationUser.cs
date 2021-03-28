#nullable disable

namespace TrashMob.Models
{
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;

    public partial class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            UsersRecruited = new HashSet<ApplicationUser>();
            UserFeedbackRegardingUsers = new HashSet<UserFeedback>();
            UserFeedbackUsers = new HashSet<UserFeedback>();
            UsersFollowing = new HashSet<UserSubscription>();
            UsersFollowed = new HashSet<UserSubscription>();
            EventsCreated = new HashSet<Event>();
            EventsUpdated = new HashSet<Event>();
            AttendeeNotifications = new HashSet<AttendeeNotification>();
        }

        public Guid UserId { get; set; }

        public DateTimeOffset? DateAgreedToPrivacyPolicy { get; set; }
        public string PrivacyPolicyVersion { get; set; }
        public DateTimeOffset? DateAgreedToTermsOfService { get; set; }
        public string TermsOfServiceVersion { get; set; }
        public string RecruitedByUserId { get; set; }
        public DateTimeOffset? MemberSince { get; set; }

        public virtual ApplicationUser RecruitedByUser { get; set; }
        public virtual ICollection<ApplicationUser> UsersRecruited { get; set; }
        public virtual ICollection<UserFeedback> UserFeedbackRegardingUsers { get; set; }
        public virtual ICollection<UserFeedback> UserFeedbackUsers { get; set; }
        public virtual ICollection<UserSubscription> UsersFollowing { get; set; }
        public virtual ICollection<UserSubscription> UsersFollowed { get; set; }
        public virtual ICollection<Event> EventsCreated { get; set; }
        public virtual ICollection<Event> EventsUpdated { get; set; }
        public virtual ICollection<AttendeeNotification> AttendeeNotifications { get; set; }
    }
}
