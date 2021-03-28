#nullable disable

namespace TrashMob.Models
{
    using System;

    public partial class AttendeeNotification
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public Guid NotificationTypeId { get; set; }

        public DateTimeOffset NotificationDate { get; set; }

        public DateTimeOffset? AcknowledgedDate { get; set; }

        public virtual Event Event { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual NotificationType NotificationType { get; set; }
    }
}
