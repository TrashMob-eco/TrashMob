#nullable disable

namespace TrashMob.Models
{
    using System;

    /// <summary>
    /// Represents a timestamped interaction note on a contact.
    /// </summary>
    public class ContactNote : KeyedModel
    {
        public Guid ContactId { get; set; }

        public int NoteType { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public virtual Contact Contact { get; set; }
    }
}
