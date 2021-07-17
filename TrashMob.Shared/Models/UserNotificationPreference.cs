#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class UserNotificationPreference
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public int UserNotificationTypeId { get; set; }

        public bool IsOptedOut { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public virtual UserNotificationType UserNotificationType { get; set; }

        public virtual User User { get; set; }
    }
}
