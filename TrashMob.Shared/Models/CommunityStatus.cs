#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class CommunityStatus : LookupModel
    {
        public CommunityStatus()
        {
            Communities = new HashSet<Community>();
        }

        public virtual ICollection<Community> Communities { get; set; }
    }
}
