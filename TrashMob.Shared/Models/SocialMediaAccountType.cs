#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class SocialMediaAccountType : LookupModel
    {
        public SocialMediaAccountType()
        {
            SocialMediaAccounts = new HashSet<SocialMediaAccount>();
        }

        public virtual ICollection<SocialMediaAccount> SocialMediaAccounts { get; set; }
    }
}
