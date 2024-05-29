#nullable disable

namespace TrashMob.Models
{
    public class UserNotificationType : LookupModel
    {
        public UserNotificationType()
        {
            UserNotifications = new HashSet<UserNotification>();
            NonEventUserNotifications = new HashSet<NonEventUserNotification>();
        }

        public virtual ICollection<UserNotification> UserNotifications { get; set; }

        public virtual ICollection<NonEventUserNotification> NonEventUserNotifications { get; set; }
    }
}