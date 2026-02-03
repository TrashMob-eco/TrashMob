#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a user's subscription preference for a newsletter category.
    /// This is a join table between User and NewsletterCategory with additional tracking fields.
    /// </summary>
    public class UserNewsletterPreference
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the newsletter category identifier.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is subscribed to this category.
        /// </summary>
        public bool IsSubscribed { get; set; } = true;

        /// <summary>
        /// Gets or sets when the user subscribed to this category.
        /// </summary>
        public DateTimeOffset? SubscribedDate { get; set; }

        /// <summary>
        /// Gets or sets when the user unsubscribed from this category.
        /// </summary>
        public DateTimeOffset? UnsubscribedDate { get; set; }

        /// <summary>
        /// Gets or sets the user navigation property.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the newsletter category navigation property.
        /// </summary>
        public virtual NewsletterCategory Category { get; set; }
    }
}
