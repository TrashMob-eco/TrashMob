#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a notification sent to a user about an event.
    /// </summary>
    public class UserNotification : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the user receiving the notification.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the event related to the notification.
        /// </summary>
        public Guid EventId { get; set; }

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
        /// Gets or sets the event associated with the notification.
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// Gets or sets the user who received the notification.
        /// </summary>
        public virtual User User { get; set; }
    }
}