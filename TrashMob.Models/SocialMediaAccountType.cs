#nullable disable

namespace TrashMob.Models
{
    public class SocialMediaAccountType : LookupModel
    {
        public SocialMediaAccountType()
        {
            PartnerSocialMediaAccounts = new HashSet<PartnerSocialMediaAccount>();
        }

        public virtual ICollection<PartnerSocialMediaAccount> PartnerSocialMediaAccounts { get; set; }
    }
}