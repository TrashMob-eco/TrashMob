#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a social media account associated with a partner organization.
    /// </summary>
    public class PartnerSocialMediaAccount : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the partner organization.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the account identifier or username on the social media platform.
        /// </summary>
        public string AccountIdentifier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this social media account is active.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the partner organization this account belongs to.
        /// </summary>
        public virtual Partner Partner { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the social media account type.
        /// </summary>
        public int SocialMediaAccountTypeId { get; set; }

        /// <summary>
        /// Gets or sets the type of social media platform.
        /// </summary>
        public virtual SocialMediaAccountType SocialMediaAccountType { get; set; }
    }
}