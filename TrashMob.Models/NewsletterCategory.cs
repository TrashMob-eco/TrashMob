#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a newsletter category that users can subscribe/unsubscribe to.
    /// Categories include sitewide newsletters, community newsletters, and team newsletters.
    /// </summary>
    public class NewsletterCategory : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewsletterCategory"/> class.
        /// </summary>
        public NewsletterCategory()
        {
            UserPreferences = new HashSet<UserNewsletterPreference>();
            Newsletters = new HashSet<Newsletter>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether new users are auto-subscribed to this category.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the collection of user preferences for this category.
        /// </summary>
        public virtual ICollection<UserNewsletterPreference> UserPreferences { get; set; }

        /// <summary>
        /// Gets or sets the collection of newsletters in this category.
        /// </summary>
        public virtual ICollection<Newsletter> Newsletters { get; set; }
    }
}
