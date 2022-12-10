#nullable disable

using TrashMob;

namespace TrashMob.Models
{
    using System.Collections.Generic;

    public partial class SocialMediaAccountType : LookupModel
    {
        public SocialMediaAccountType()
        {
            PartnerSocialMediaAccounts = new HashSet<PartnerSocialMediaAccount>();
        }

        public virtual ICollection<PartnerSocialMediaAccount> PartnerSocialMediaAccounts { get; set; }
    }
}
