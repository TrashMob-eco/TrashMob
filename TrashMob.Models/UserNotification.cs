﻿#nullable disable

namespace TrashMob.Models
{
    public class UserNotification : KeyedModel
    {
        public Guid UserId { get; set; }

        public Guid EventId { get; set; }

        public int UserNotificationTypeId { get; set; }

        public DateTimeOffset? SentDate { get; set; }

        public virtual UserNotificationType UserNotificationType { get; set; }

        public virtual Event Event { get; set; }

        public virtual User User { get; set; }
    }
}