#nullable disable

namespace TrashMob.Models
{
    using System;

    /// <summary>
    /// Junction table linking contacts to tags (many-to-many).
    /// </summary>
    public class ContactContactTag : BaseModel
    {
        public Guid ContactId { get; set; }

        public Guid ContactTagId { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual ContactTag ContactTag { get; set; }
    }
}
