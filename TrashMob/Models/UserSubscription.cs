#nullable disable

namespace TrashMob.Models
{
    using System;

    public partial class UserSubscription
    {
        public Guid UserId { get; set; }
        public Guid FollowingId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser FollowingUser { get; set; }
    }
}
