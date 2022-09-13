#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class MediaUsageType : LookupModel
    {
        public MediaUsageType()
        {
            EventMedias = new HashSet<EventMedia>();
        }

        public virtual ICollection<EventMedia> EventMedias { get; set; }
    }
}
