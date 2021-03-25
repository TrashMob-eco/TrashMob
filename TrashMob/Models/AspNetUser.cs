namespace TrashMob.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public partial class AspNetUser : IdentityUser
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
