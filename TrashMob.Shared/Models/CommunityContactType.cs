#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class CommunityContactType : LookupModel
    {
        public CommunityContactType()
        {
            CommunityContacts = new HashSet<CommunityContact>();
        }

        public virtual ICollection<CommunityContact> CommunityContacts { get; set; }
   }
}
