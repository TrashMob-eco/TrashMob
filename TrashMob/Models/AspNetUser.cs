using System;
using System.Collections.Generic;

#nullable disable

namespace TrashMob.Models
{
    public partial class AspNetUser
    {
        public AspNetUser()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetUserRoles = new HashSet<AspNetUserRole>();
            AspNetUserTokens = new HashSet<AspNetUserToken>();
            EventCreatedByUsers = new HashSet<Event>();
            EventLastUpdatedByUsers = new HashSet<Event>();
            UserDetailRecruitedByUsers = new HashSet<UserDetail>();
            UserFeedbackRegardingUsers = new HashSet<UserFeedback>();
            UserFeedbackUsers = new HashSet<UserFeedback>();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        public virtual UserDetail UserDetailUser { get; set; }
        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual ICollection<Event> EventCreatedByUsers { get; set; }
        public virtual ICollection<Event> EventLastUpdatedByUsers { get; set; }
        public virtual ICollection<UserDetail> UserDetailRecruitedByUsers { get; set; }
        public virtual ICollection<UserFeedback> UserFeedbackRegardingUsers { get; set; }
        public virtual ICollection<UserFeedback> UserFeedbackUsers { get; set; }
    }
}
