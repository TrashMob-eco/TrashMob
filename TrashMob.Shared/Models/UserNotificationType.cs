#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class UserNotificationType
    {
        public UserNotificationType()
        {
            UserNotifications = new HashSet<UserNotification>();
        }

        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int? DisplayOrder { get; set; }
        
        public bool? IsActive { get; set; }

        public virtual ICollection<UserNotification> UserNotifications { get; set; }

        public virtual ICollection<UserNotificationPreference> UserNotificationPreferences { get; set; }
    }
}
