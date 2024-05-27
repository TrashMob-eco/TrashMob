#nullable disable

namespace TrashMob.Models
{
    public class NonEventUserNotification : KeyedModel
    {
        public Guid UserId { get; set; }

        public int UserNotificationTypeId { get; set; }

        public DateTimeOffset? SentDate { get; set; }

        public virtual UserNotificationType UserNotificationType { get; set; }

        public virtual User User { get; set; }
    }
}