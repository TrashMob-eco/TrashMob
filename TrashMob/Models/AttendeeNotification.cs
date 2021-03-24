using System;
using System.Collections.Generic;

#nullable disable

namespace TrashMob.Models
{
    public partial class AttendeeNotification
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string UserId { get; set; }
        public Guid NotificationTypeId { get; set; }
        public DateTimeOffset NotificationDate { get; set; }
        public DateTimeOffset? AcknowledgedDate { get; set; }
    }
}
