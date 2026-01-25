#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a notification sent to a user that is not related to a specific event.
    /// </summary>
    public class NonEventUserNotification : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the user receiving the notification.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the notification type.
        /// </summary>
        public int UserNotificationTypeId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the notification was sent.
        /// </summary>
        public DateTimeOffset? SentDate { get; set; }

        /// <summary>
        /// Gets or sets the type of notification.
        /// </summary>
        public virtual UserNotificationType UserNotificationType { get; set; }

        /// <summary>
        /// Gets or sets the user who received the notification.
        /// </summary>
        public virtual User User { get; set; }
    }
}