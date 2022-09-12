#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class MediaType : LookupModel
    {
        public MediaType()
        {
            EventMedias = new HashSet<EventMedia>();
        }

        public virtual ICollection<EventMedia> EventMedias { get; set; }
    }
}
