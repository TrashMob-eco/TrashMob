using System;
using System.Collections.Generic;

#nullable disable

namespace TrashMob.Models
{
    public partial class UserFeedback
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid EventId { get; set; }
        public string RegardingUserId { get; set; }
        public int? EventRating { get; set; }
        public int? UserRating { get; set; }
        public string Comments { get; set; }

        public virtual AspNetUser RegardingUser { get; set; }
        public virtual AspNetUser User { get; set; }
    }
}
