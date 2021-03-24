using System;
using System.Collections.Generic;

#nullable disable

namespace TrashMob.Models
{
    public partial class UserSubscription
    {
        public string UserId { get; set; }
        public string UserIdFollowing { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        public virtual AspNetUser User { get; set; }
        public virtual AspNetUser UserIdFollowingNavigation { get; set; }
    }
}
