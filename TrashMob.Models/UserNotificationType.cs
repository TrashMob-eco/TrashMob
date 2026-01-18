#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a type of user notification (e.g., event reminders, summaries).
    /// </summary>
    public class UserNotificationType : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationType"/> class.
        /// </summary>
        public UserNotificationType()
        {
            UserNotifications = new HashSet<UserNotification>();
            NonEventUserNotifications = new HashSet<NonEventUserNotification>();
        }

        /// <summary>
        /// Gets or sets the collection of user notifications of this type.
        /// </summary>
        public virtual ICollection<UserNotification> UserNotifications { get; set; }

        /// <summary>
        /// Gets or sets the collection of non-event user notifications of this type.
        /// </summary>
        public virtual ICollection<NonEventUserNotification> NonEventUserNotifications { get; set; }
    }
}