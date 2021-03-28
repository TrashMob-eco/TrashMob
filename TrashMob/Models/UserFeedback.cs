#nullable disable

namespace TrashMob.Models
{
    using System;

    public partial class UserFeedback
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public Guid RegardingUserId { get; set; }
        public int? EventRating { get; set; }
        public int? UserRating { get; set; }
        public string Comments { get; set; }

        public virtual ApplicationUser RegardingUser { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Event Event { get; set; }
    }
}
