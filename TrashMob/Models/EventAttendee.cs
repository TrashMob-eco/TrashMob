using System;
using System.Collections.Generic;

#nullable disable

namespace TrashMob.Models
{
    public partial class EventAttendee
    {
        public Guid EventId { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset SignUpDate { get; set; }
        public DateTimeOffset? CanceledDate { get; set; }
        public DateTimeOffset? ArrivalTime { get; set; }
        public DateTimeOffset? DepartureTime { get; set; }

        public virtual Event Event { get; set; }
        public virtual AspNetUser User { get; set; }
    }
}
