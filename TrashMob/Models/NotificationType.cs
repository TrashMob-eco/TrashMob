#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    public partial class NotificationType
    {
        public NotificationType()
        {
            AttendeeNotifications = new HashSet<AttendeeNotification>();
        }

        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }

        public virtual ICollection<AttendeeNotification> AttendeeNotifications { get; set; }
    }
}
