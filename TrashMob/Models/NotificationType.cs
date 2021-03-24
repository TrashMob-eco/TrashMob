using System;
using System.Collections.Generic;

#nullable disable

namespace TrashMob.Models
{
    public partial class NotificationType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
