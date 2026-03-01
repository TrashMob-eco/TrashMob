#nullable disable

namespace TrashMob.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a tag used for categorizing and segmenting contacts.
    /// </summary>
    public class ContactTag : KeyedModel
    {
        public ContactTag()
        {
            ContactContactTags = [];
        }

        public string Name { get; set; }

        public string Color { get; set; }

        public virtual ICollection<ContactContactTag> ContactContactTags { get; set; }
    }
}
