#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a type of social media platform (e.g., Facebook, Twitter, Instagram).
    /// </summary>
    public class SocialMediaAccountType : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SocialMediaAccountType"/> class.
        /// </summary>
        public SocialMediaAccountType()
        {
            PartnerSocialMediaAccounts = new HashSet<PartnerSocialMediaAccount>();
        }

        /// <summary>
        /// Gets or sets the collection of partner social media accounts of this type.
        /// </summary>
        public virtual ICollection<PartnerSocialMediaAccount> PartnerSocialMediaAccounts { get; set; }
    }
}